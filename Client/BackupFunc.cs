using ClientDemon.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
                    if (isTime(sbackup.backup_time) && sbackup.last_possible_backup_date > DateTime.Now)
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
                Console.WriteLine(a);
                if (a < 60 && a > 0) //backup za poslední min
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
                        Full_Backup(source, target);
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
                throw new NotImplementedException();

        }
        private void Full_Backup(backup_source source, backup_target target)
        {
            target.config += @"\full\";
            if (!Directory.Exists(target.config))
                Directory.CreateDirectory(target.config);
            if (Directory.GetDirectories(target.config).Length > 4) //smazat nejstarší
            {
                FileSystemInfo fileInfo = new DirectoryInfo(target.config).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                Directory.Delete(fileInfo.FullName, true);
            }

                    
            Copy(source.path, target.config + DateTime.Now.ToString("yyyy/MM/dd H.mm")); //prozatim v config

            CreateSnapshot(source.path, target.config + DateTime.Now.ToString("yyyy/MM/dd H.mm"));
        }

        private void Diff_Backup(backup_source source, backup_target target)
        {
            target.config += @"\diff\";
            if (Directory.GetDirectories(target.config).Length == 0) //pokud je první 
            {
                if (!Directory.Exists(target.config))
                    Directory.CreateDirectory(target.config);

                Copy(source.path, target.config + DateTime.Now.ToString("yyyy/MM/dd H.mm")); //prozatim v config

                CreateSnapshot(source.path, target.config + DateTime.Now.ToString("yyyy/MM/dd H.mm"));
            }
            else //pokud není první
            {
                //zkopírovat jen soubory který nejsou v prvním snapshotu
                //do snapshotu dát všechny
       
            }
        }
        private void Inc_Backup(fullBackupInfo backup)
        {

        }


        public void Copy(string source, string target)
         {

            DirectoryInfo Source = new DirectoryInfo(source);
            DirectoryInfo Target = new DirectoryInfo(target);

            CopyDirectories(Source, Target);
             
         }
         public void CopyDirectories(DirectoryInfo source, DirectoryInfo target)
         {
            Directory.CreateDirectory(target.FullName);

            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }

            foreach (DirectoryInfo SourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(SourceSubDir.Name);
                CopyDirectories(SourceSubDir, nextTargetSubDir);
            }
        }

        public void CreateSnapshot(string ssource, string ttarget)
        {
            DirectoryInfo source = new DirectoryInfo(ssource);
            DirectoryInfo target = new DirectoryInfo(ttarget);

            string filepath = target + @"\snapshot.txt";
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
                s.change = SourceSubDir.CreationTime.ToString();
                s.path = SourceSubDir.FullName;
                s.type = "folder";

                snapshots.Add(s);
            }

            using (StreamWriter file = File.CreateText(filepath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, snapshots);
            }
        }
    }
}
