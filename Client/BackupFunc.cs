using ClientDemon.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon
{
    public class BackupFunc
    {
        public void DoBackup()
        {
            fullBackupInfo[] backups = FileConfig.GetBackups();

            foreach (fullBackupInfo sbackup in backups)
            {
                if (sbackup.active) //jeste zkontrolovat jestli je právě čas
                {
                    if (sbackup.backup_type == "full_backup")
                        Full_Backup(sbackup);
                    else if (sbackup.backup_type == "diff_backup")
                        Diff_Backup(sbackup);
                    else if (sbackup.backup_type == "inc_backup")
                        Inc_Backup(sbackup);
                }
            }
        }
        private void Full_Backup(fullBackupInfo backup)
        {
            foreach (backup_source source in backup.backup_source)
            {
                foreach (backup_target target in backup.backup_target)
                {
                    if (System.IO.Directory.GetDirectories(target.config).Length > 4) //smazat nejstarší
                    {
                        FileSystemInfo fileInfo = new DirectoryInfo(target.config).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                        Directory.Delete(fileInfo.FullName, true);
                    }

                    Copy(source.path, target.config + DateTime.Now.ToString("yyyy/MM/dd H.mm")); //prozatim v config
                }
            }

        }
        private void Diff_Backup(fullBackupInfo backup)
        {

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
    }
}
