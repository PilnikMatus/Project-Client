using ClientDemon.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ClientDemon
{
    public class BackupFunc
    {
        public void CheckBackup()
        {
            fullBackupInfo[] backups = FileConfig.GetBackups();

            foreach (fullBackupInfo sbackup in backups)
            {
                if (sbackup.active)
                {
                    //if (sbackup.last_possible_backup_date > DateTime.Now && isTime(sbackup.backup_time)) //zjištění času
                        DoBackup(sbackup);
                }
            }
        }
        private bool isTime(List<backup_time> times)
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
            foreach (backup_source source in sbackup.backup_source)
            {
                foreach (backup_target target in sbackup.backup_target)
                {
                    if (sbackup.backup_type == "full_backup")
                        Full_Backup(sbackup.format_type, source, target);
                    if (sbackup.backup_type == "diff_backup")
                        Diff_Backup(sbackup.format_type, source, target);
                    if (sbackup.backup_type == "inc_backup")
                        Inc_Backup(sbackup.format_type, source, target);
                }
            }
        }
        private void Full_Backup(string format_type, backup_source source, backup_target target)
        {
            string targetPath = target.path;
            targetPath += @"\full\";
            CheckParentDirectories(targetPath);



            if (format_type == "zip")
                ZipFile.CreateFromDirectory(source.path, targetPath + DateTime.Now.ToString("yyyy/MM/dd H.mm") + ".zip"); //prozatim v config
            else
                Copy(source.path, targetPath + DateTime.Now.ToString("yyyy/MM/dd H.mm")); //prozatim v config
        }

        private void Diff_Backup(string format_type, backup_source source, backup_target target)
        {
            string targetPath = target.path;
            targetPath += @"\diff\";
            string targetp = targetPath;
            CheckParentDirectories(targetPath);

            if (Directory.GetDirectories(targetPath).Length > 0)
            {
                FileSystemInfo fileInfo2 = new DirectoryInfo(targetPath).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).Last();
                if (Directory.GetDirectories(fileInfo2.FullName).Length < 5)
                    targetPath = fileInfo2.FullName;
                else
                {
                    targetPath += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
                    Directory.CreateDirectory(targetPath);
                }
            } 
            else if (Directory.GetDirectories(targetPath).Length == 0)
            {
                targetPath += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
                Directory.CreateDirectory(targetPath);
            }
            
            if (Directory.GetDirectories(targetPath).Length == 0 && Directory.GetFiles(targetPath).Length == 0)
            {
                SaveSnapshot(source.path, targetPath);

                targetPath += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");

                if (format_type == "zip")
                    ZipFile.CreateFromDirectory(source.path, targetPath + ".zip"); //prozatim v config
                else
                    Copy(source.path, targetPath); //prozatim v config
            } 
            else
            {
                List<snapshot> rs = ReadPreviousSnapshot(targetPath);
                List<snapshot> cs = GetSnapshotList(source.path);

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

                DeletedSave(targetPath, deleted);

                if (format_type == "zip")
                    ZipFile.CreateFromDirectory(source.path, targetPath + @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm") + ".zip"); //prozatim v config
                else
                    Copy(source.path, targetPath + @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm"), difference); //prozatim v config

            }
        }
        private void Inc_Backup(string format_type, backup_source source, backup_target target)
        {
            string targetPath = target.path;
            targetPath += @"\inc\";
            string targetp = targetPath;
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            if (Directory.GetDirectories(targetPath).Length > 4) //smazat nejstarší - max 5
            {
                FileSystemInfo fileInfo = new DirectoryInfo(targetPath).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                Directory.Delete(fileInfo.FullName, true);
            }

            if (Directory.GetDirectories(targetPath).Length > 0)
            {
                FileSystemInfo fileInfo2 = new DirectoryInfo(targetPath).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).Last();
                if (Directory.GetDirectories(fileInfo2.FullName).Length < 5)
                    targetPath = fileInfo2.FullName;
                else
                {
                    targetPath += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
                    Directory.CreateDirectory(targetPath);
                }
            }
            else if (Directory.GetDirectories(targetPath).Length == 0)
            {
                targetPath += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
                Directory.CreateDirectory(targetPath);
            }

            if (Directory.GetDirectories(targetPath).Length == 0) //pokud je první
            {
                SaveSnapshot(source.path, targetPath);

                targetPath += @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
                Copy(source.path, targetPath); //prozatim v config
            }
            else
            {
                List<snapshot> rs = ReadPreviousSnapshot(targetPath);
                List<snapshot> cs = GetSnapshotList(source.path);

                SaveSnapshot(source.path, targetPath);

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

                DeletedSave(targetPath, deleted);

                if (format_type == "zip")
                    ZipFile.CreateFromDirectory(source.path, targetPath + @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm") + ".zip"); //prozatim v config
                else
                    Copy(source.path, targetPath + @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm"), difference); //prozatim v config
            }
        }
        private void CheckParentDirectories(string target)
        {
            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);
            if (Directory.GetDirectories(target).Length > 4) //smazat nejstarší - max 5
            {
                FileSystemInfo fileInfo = new DirectoryInfo(target).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                Directory.Delete(fileInfo.FullName, true);
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
        private void SaveSnapshot(string ssource, string ttarget)
        {
            DirectoryInfo target = new DirectoryInfo(ttarget);

            string filepath = target + @"\snapshot.txt";

            List<snapshot> snapshots = GetSnapshotList(ssource);

            using (StreamWriter file = File.CreateText(filepath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, snapshots);
            }
        }
        private List<snapshot> GetSnapshotList(string ssource)
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

                snapshots.AddRange(GetSnapshotList(SourceSubDir.FullName));
            }
            return snapshots;
        }
        private List<snapshot> ReadPreviousSnapshot(string target)
        {
            FileSystemInfo fileInfo = new DirectoryInfo(target).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();

            using (StreamReader file = new StreamReader(target + @"\snapshot.txt"))
            {
                List<snapshot> snapshots = JsonConvert.DeserializeObject<snapshot[]>(file.ReadToEnd()).ToList();
                return snapshots;
            }
        }
    }
}
