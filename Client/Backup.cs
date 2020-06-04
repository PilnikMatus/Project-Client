using ClientDemon.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ClientDemon
{
    public class Backup
    {
        private static SnapshotFunc snapshotFunc = new SnapshotFunc();
        private static FileFunc fileFunc = new FileFunc();
        public void CheckBackup()
        {
            fullBackupInfo[] backups = ClientConfig.GetBackups();

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
                        Full_Backup(sbackup, source, target);
                    if (sbackup.backup_type == "diff_backup")
                        Diff_Backup(sbackup, source, target);
                    if (sbackup.backup_type == "inc_backup")
                        Inc_Backup(sbackup, source, target);
                }
            }
        }
        private void Full_Backup(fullBackupInfo backup, backup_source source, backup_target target)
        {
            string targetPath = target.path;
            targetPath += @"\full\";

            bool ZIP = backup.format_type == "zip";

            fileFunc.CheckParentDirectory(targetPath, ZIP);

            if (ZIP)
                ZipFile.CreateFromDirectory(source.path, targetPath + AddDate() + ".zip");
            else
                fileFunc.Copy(source.path, targetPath + AddDate());

            fileFunc.DeleteOldBackups(target.path + @"\full\", ZIP);

            HttpRequests.PostJobHistory(backup.id, DateTime.Now, true);
        }

        private void Diff_Backup(fullBackupInfo backup, backup_source source, backup_target target)
        {
            string targetPath = target.path;
            targetPath += @"\diff\";

            bool ZIP = backup.format_type == "zip";

            targetPath = fileFunc.CheckDirectoriesGetRealName(targetPath, ZIP);

            if (Directory.GetDirectories(targetPath).Length == 0 && Directory.GetFiles(targetPath).Length == 0)
            {
                snapshotFunc.SaveSnapshot(source.path, targetPath);

                targetPath += AddDate();

                if (ZIP)
                    ZipFile.CreateFromDirectory(source.path, targetPath + ".zip");
                else
                    fileFunc.Copy(source.path, targetPath);
            } 
            else
            {
                List<snapshot> rs = snapshotFunc.ReadPreviousSnapshot(targetPath); //last snapshot
                List<snapshot> cs = snapshotFunc.GetSnapshotList(source.path); //current snapshot

                snapshotFunc.SaveDeleted(rs, cs, targetPath);
                List<snapshot> difference = snapshotFunc.GetSnapshotDifference(rs, cs);

                if (ZIP)
                {
                    string targetPathZ = targetPath + AddDate();

                    fileFunc.Copy(source.path, targetPathZ, difference);
                    ZipFile.CreateFromDirectory(targetPathZ, targetPathZ + ".zip");
                    Directory.Delete(targetPathZ, true);
                }
                else
                    fileFunc.Copy(source.path, targetPath + AddDate(), difference);
            }

            fileFunc.DeleteOldBackups(target.path + @"\diff\", ZIP);


        }

        private void Inc_Backup(fullBackupInfo backup, backup_source source, backup_target target)
        {
            string targetPath = target.path;
            targetPath += @"\inc\";

            bool ZIP = backup.format_type == "zip";

            targetPath = fileFunc.CheckDirectoriesGetRealName(targetPath, ZIP);

            if (Directory.GetDirectories(targetPath).Length == 0 && Directory.GetFiles(targetPath).Length == 0)
            {
                snapshotFunc.SaveSnapshot(source.path, targetPath);

                targetPath += AddDate();

                if (ZIP)
                    ZipFile.CreateFromDirectory(source.path, targetPath + ".zip");
                else
                    fileFunc.Copy(source.path, targetPath);
            }
            else
            {
                List<snapshot> rs = snapshotFunc.ReadPreviousSnapshot(targetPath);
                List<snapshot> cs = snapshotFunc.GetSnapshotList(source.path);

                snapshotFunc.SaveSnapshot(source.path, targetPath);

                snapshotFunc.SaveDeleted(rs, cs, targetPath);
                List<snapshot> difference = snapshotFunc.GetSnapshotDifference(rs, cs);

                if (ZIP)
                {
                    string targetPathZ = targetPath + AddDate();

                    fileFunc.Copy(source.path, targetPathZ, difference);
                    ZipFile.CreateFromDirectory(targetPathZ, targetPathZ + ".zip");
                    Directory.Delete(targetPathZ, true);
                }
                else
                    fileFunc.Copy(source.path, targetPath + AddDate(), difference);
            }

            fileFunc.DeleteOldBackups(target.path + @"\inc\", ZIP);
        }
        private string AddDate()
        {
            return @"\" + DateTime.Now.ToString("yyyy/MM/dd H.mm");
        }
    }
}
