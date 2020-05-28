using ClientDemon.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace ClientDemon
{
    public class BackupFunc
    {
        public void CheckBackup()
        {
            fullBackupInfo[] backups = FileConfig.GetBackups();

            foreach (fullBackupInfo sbackup in backups)
            {
                if (sbackup.active) //jeste zkontrolovat jestli je právě čas
                {
                    //if (sbackup.last_possible_backup_date > DateTime.Now && isTime(sbackup.backup_time)) //zjištění jestli mám teď dělat zálohu
                        DoBackup(sbackup);
                }
            }
        }
        private bool isTime(List<backup_time> times) //zjistnei času u tohoto jednoho backup
        {
            int repetation;
            foreach (backup_time item in times)
            {
                if (item.repetation_unit == "hour")
                    repetation = item.repetation_number * 60 * 60;
                else if (item.repetation_unit == "day")
                    repetation = item.repetation_number * 24 * 60 * 60;
                else if (item.repetation_unit == "week")
                    repetation = item.repetation_number * 7 * 24 * 60 * 60;
                else// (item.repetation_unit == "month")
                    repetation = item.repetation_number * 30 * 24 * 60 * 60;

                double seconds = (DateTime.Now - item.start_time).TotalSeconds;
                double a = seconds % Convert.ToDouble(repetation);
                if (a < 60 && a > 0) //byl backup za poslední min?
                {
                    Console.WriteLine("právě zálohuji");
                    return true;
                }
                
            }
            return false;
        }
        private void DoBackup(fullBackupInfo sbackup)
        {
            if (sbackup.backup_type == "full_backup")
            {
                foreach (backup_source source in sbackup.backup_source)
                {
                    foreach (backup_target target in sbackup.backup_target)
                    {
                        Full_Backup(sbackup, source, target);
                    }
                }
            }
            else if (sbackup.backup_type == "diff_backup")
            {
                foreach (backup_source source in sbackup.backup_source)
                {
                    foreach (backup_target target in sbackup.backup_target)
                    {
                        Diff_Backup(source, target);
                    }
                }
            }
            else if (sbackup.backup_type == "inc_backup")
            {
                foreach (backup_source source in sbackup.backup_source)
                {
                    foreach (backup_target target in sbackup.backup_target)
                    {
                        Inc_Backup(source, target);
                    }
                }
            }

        }
        private void Full_Backup(fullBackupInfo backup, backup_source source, backup_target target)
        {
            target.config += @"\full\";
            if (!Directory.Exists(target.config))
                Directory.CreateDirectory(target.config);
            if (Directory.GetDirectories(target.config).Length > 4) //smazat nejstarší - max 5
            {
                FileSystemInfo fileInfo = new DirectoryInfo(target.config).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                Directory.Delete(fileInfo.FullName, true);
            }

            if(backup.format_type == "zip")
                ZipFile.CreateFromDirectory(source.path, target.config + DateTime.Now.ToString("yyyy/MM/dd H.mm") + ".zip"); //prozatim v config
            else
                Copy(source.path, target.config + DateTime.Now.ToString("yyyy/MM/dd H.mm")); //prozatim v config
        }

        private void Diff_Backup(backup_source source, backup_target target)
        {
            target.config += @"\diff\";
            string targetp = target.config;
            if (!Directory.Exists(target.config))
                Directory.CreateDirectory(target.config);

            if (Directory.GetDirectories(target.config).Length > 4) //smazat nejstarší - max 5
            {
                FileSystemInfo fileInfo = new DirectoryInfo(target.config).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                Directory.Delete(fileInfo.FullName, true);
            } 

            if (Directory.GetDirectories(target.config).Length > 0)
            {
                FileSystemInfo fileInfo2 = new DirectoryInfo(target.config).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).Last();
                if (Directory.GetDirectories(fileInfo2.FullName).Length < 5)
                    target.config = fileInfo2.FullName;
                else
                {
                    target.config += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
                    Directory.CreateDirectory(target.config);
                }
            } 
            else if (Directory.GetDirectories(target.config).Length == 0)
            {
                target.config += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
                Directory.CreateDirectory(target.config);
            }

            if (Directory.GetDirectories(target.config).Length == 0) //pokud je první nwbo vic nez 4
            {
                CreateSnapshot(source.path, target.config);

                target.config += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
                Copy(source.path, target.config); //prozatim v config
            } 
            else
            {
                List<snapshot> rs = ReadPreviousSnapshot(target.config);
                List<snapshot> cs = CreateSnapshot2(source.path);

                bool pokus = false;
                List<snapshot> difference = new List<snapshot>();
                foreach (snapshot item1 in cs)
                {
                    pokus = false;
                    foreach (snapshot item2 in rs)
                    {
                        if (item1.path == item2.path &&
                           item1.type == item2.type &&
                           item1.change == item2.change)
                            pokus = true;
                    }
                    if (!pokus)
                        difference.Add(item1);
                }
                foreach (snapshot item in difference)
                {
                    Console.WriteLine(item.path);
                }

                List<snapshot> deleted = new List<snapshot>();
                foreach (snapshot item1 in rs)
                {
                    pokus = false;
                    foreach (snapshot item2 in cs)
                    {
                        if (item1.path == item2.path &&
                           item1.type == item2.type &&
                           item1.change == item2.change)
                            pokus = true;
                    }
                    if (!pokus)
                        deleted.Add(item1);
                }

                DeletedSave(target.config, deleted);
                Copy(source.path, target.config + @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm"), difference); //prozatim v config
            }
        }
        private void Inc_Backup(backup_source source, backup_target target)
        {
            target.config += @"\inc\";
            string targetp = target.config;
            if (!Directory.Exists(target.config))
                Directory.CreateDirectory(target.config);

            if (Directory.GetDirectories(target.config).Length > 4) //smazat nejstarší - max 5
            {
                FileSystemInfo fileInfo = new DirectoryInfo(target.config).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                Directory.Delete(fileInfo.FullName, true);
            }

            if (Directory.GetDirectories(target.config).Length > 0)
            {
                FileSystemInfo fileInfo2 = new DirectoryInfo(target.config).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).Last();
                if (Directory.GetDirectories(fileInfo2.FullName).Length < 5)
                    target.config = fileInfo2.FullName;
                else
                {
                    target.config += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
                    Directory.CreateDirectory(target.config);
                }
            }
            else if (Directory.GetDirectories(target.config).Length == 0)
            {
                target.config += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
                Directory.CreateDirectory(target.config);
            }

            if (Directory.GetDirectories(target.config).Length == 0) //pokud je první
            {
                CreateSnapshot(source.path, target.config);

                target.config += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
                Copy(source.path, target.config); //prozatim v config
            }
            else
            {
                List<snapshot> rs = ReadPreviousSnapshot(target.config);
                List<snapshot> cs = CreateSnapshot2(source.path);

                CreateSnapshot(source.path, target.config);

                bool pokus = false;
                List<snapshot> difference = new List<snapshot>();
                foreach (snapshot item1 in cs)
                {
                    pokus = false;
                    foreach (snapshot item2 in rs)
                    {
                        if (item1.path == item2.path &&
                           item1.type == item2.type &&
                           item1.change == item2.change)
                            pokus = true;
                    }
                    if (!pokus)
                        difference.Add(item1);
                }

                List<snapshot> deleted = new List<snapshot>();
                foreach (snapshot item1 in rs)
                {
                    pokus = false;
                    foreach (snapshot item2 in cs)
                    {
                        if (item1.path == item2.path &&
                           item1.type == item2.type &&
                           item1.change == item2.change)
                            pokus = true;
                    }
                    if (!pokus)
                        deleted.Add(item1);
                }

                DeletedSave(target.config, deleted);
                Copy(source.path, target.config + @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm"), difference); //prozatim v config
            }
        }

        public void Copy(string source, string target, List<snapshot> previousSnapshot = null)
         {

            DirectoryInfo Source = new DirectoryInfo(source);
            DirectoryInfo Target = new DirectoryInfo(target);

            CopyDirectories(Source, Target, previousSnapshot);
             
         }
         public void CopyDirectories(DirectoryInfo source, DirectoryInfo target, List<snapshot> previousSnapshot = null)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (FileInfo file in source.GetFiles())
            {
                if(previousSnapshot != null)
                {
                    foreach (snapshot item in previousSnapshot)
                    {
                        if (item.path == file.FullName)
                            file.CopyTo(Path.Combine(target.FullName, file.Name), true);
                    }
                }
                else
                    file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }

            foreach (DirectoryInfo SourceSubDir in source.GetDirectories())
            {
                if (previousSnapshot != null)
                {
                    foreach (snapshot item in previousSnapshot)
                    {
                        if (item.path == SourceSubDir.FullName)
                        {
                            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(SourceSubDir.Name);
                            CopyDirectories(SourceSubDir, nextTargetSubDir, previousSnapshot);
                        }
                    }
                }
                else
                {
                    DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(SourceSubDir.Name);
                    CopyDirectories(SourceSubDir, nextTargetSubDir, previousSnapshot);
                }
            }
        }
        private void DeletedSave(string ttarget, List<snapshot> deleted)
        {

            DirectoryInfo target = new DirectoryInfo(ttarget);

            string filepath = target + @"\deleted.txt";

            if(File.Exists(filepath))
            deleted.AddRange(ReadDeleted(filepath));

            using (StreamWriter file = File.CreateText(filepath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, deleted);
            }
        }
        private List<snapshot> ReadDeleted(string filepath)
        {

            using (StreamReader file = new StreamReader(filepath))
            {
                List<snapshot> deletedold = JsonConvert.DeserializeObject<snapshot[]>(file.ReadToEnd()).ToList();
                return deletedold;
            }

        }
        private void CreateSnapshot(string ssource, string ttarget)
        {
            DirectoryInfo target = new DirectoryInfo(ttarget);

            string filepath = target + @"\snapshot.txt";
            List<snapshot> snapshots = new List<snapshot>();

            snapshots = CreateSnapshot2(ssource);

            using (StreamWriter file = File.CreateText(filepath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, snapshots);
            }
        }
        private List<snapshot> CreateSnapshot2(string ssource)
        {
            DirectoryInfo source = new DirectoryInfo(ssource);

            List<snapshot> snapshots = new List<snapshot>();

            foreach (FileInfo file in source.GetFiles())
            {
                snapshot s = new snapshot();
                s.change = file.LastWriteTime.ToString();
                s.path = file.FullName;
                s.type = "file";

                snapshots.Add(s);
            }

            foreach (DirectoryInfo SourceSubDir in source.GetDirectories())
            {
                snapshot s = new snapshot();
                s.change = SourceSubDir.LastWriteTime.ToString();
                s.path = SourceSubDir.FullName;
                s.type = "folder";

                snapshots.Add(s);

                snapshots.AddRange(CreateSnapshot2(SourceSubDir.FullName));
            }
            return snapshots;
        }
        private List<snapshot> ReadPreviousSnapshot(string target)
        {
            FileSystemInfo fileInfo = new DirectoryInfo(target).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();

            using (StreamReader file = new StreamReader(target + @"\snapshot.txt"))
            {
                List<snapshot> snapshots = JsonConvert.DeserializeObject<snapshot[]>(file.ReadToEnd()).ToList();
                Console.WriteLine(snapshots.Count);
                return snapshots;
            }
        }
    }
}
