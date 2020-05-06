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
        public List<backup_time> times { get; set; }
        public List<backup_source> sources { get; set; }
        public List<backup_target> targets { get; set; }
        
        public void DoBackup()
        {
            Backup[] backups = RegistryUsing.GetBackups();

            foreach (Backup sbackup in backups)
            {
                if (sbackup.active) //jeste zkontrolovat jestli je právě čas
                {
                    GetBackupDetails(sbackup.id);

                    if (sbackup.backup_type == "full_backup")
                        Full_Backup(sbackup);
                    else if (sbackup.backup_type == "diff_backup")
                        Diff_Backup(sbackup);
                    else if (sbackup.backup_type == "inc_backup")
                        Inc_Backup(sbackup);
                }
            }
        }
        private void Full_Backup(Backup backup)
        {
            //source: c:users/Dan/Desktop
            //target: c:backup
            //target: c:backup2
            foreach (backup_source source in sources)
            {
                foreach (backup_target target in targets)
                {
                    Copy(source.path, target.config); //prozatim v config
                }
            }

        }
        private void Diff_Backup(Backup backup)
        {

        }
        private void Inc_Backup(Backup backup)
        {

        }

        private void GetBackupDetails(int id)
        {
            List<backup_source> currentBackupSources = new List<backup_source>();
            foreach (backup_source source in RegistryUsing.GetBackupSources())
            {
                if (source.id_backup == id)
                    currentBackupSources.Add(source);
            }
            this.sources = currentBackupSources;


            List<backup_target> currentBackupTargets = new List<backup_target>();
            foreach (backup_target target in RegistryUsing.GetBackupTargets())
            {
                if (target.id_backup == id)
                    currentBackupTargets.Add(target);
            }
            this.targets = currentBackupTargets;


            List<backup_time> currentBackupTimes = new List<backup_time>();
            foreach (backup_time time in RegistryUsing.GetBackupTimes())
            {
                if (time.id_backup == id)
                    currentBackupTimes.Add(time);
            }
            this.times = currentBackupTimes;

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
