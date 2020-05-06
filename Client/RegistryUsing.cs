using ClientDemon.Tables;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon
{
    public static class RegistryUsing
    {
        public static string CreateRegistryId(string id_client)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user?

            key.SetValue("pc_id", id_client);
            key.Close();

            return id_client;
        }
        public static string GetRegistryId()
        {
            RegistryKey keyread = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user?
            string id_client = "not set";

            if (keyread != null)
            {
                if (keyread.GetValue("pc_id") != null)
                    id_client = keyread.GetValue("pc_id").ToString();
                keyread.Close();
            }
            return id_client;
        }
        public static void CreateBackups(Backup[] backups) //vloží backupy do registru //není omezená délka?
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user?

            string JSONBackup = JsonConvert.SerializeObject(backups);

            key.SetValue("Backups", JSONBackup);
            key.Close();
        }
        public static Backup[] GetBackups() //bere backupy z registru
        {
            RegistryKey keyread = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user?

            if (keyread != null)
            {
                string JSONBackup = keyread.GetValue("Backups").ToString();

                Backup[] backups = JsonConvert.DeserializeObject<Backup[]>(JSONBackup);

                keyread.Close();

                return backups;
            }

            throw new Exception("Keyread was null");
        }

        public static void CreateBackupTimes(backup_time[] backups) //vloží backupy do registru //není omezená délka?
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user?

            string JSONBackup = JsonConvert.SerializeObject(backups);

            key.SetValue("BackupTimes", JSONBackup);
            key.Close();
        }
        public static backup_time[] GetBackupTimes() //bere backupy z registru
        {
            RegistryKey keyread = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user?

            if (keyread != null)
            {
                string JSONBackup = keyread.GetValue("BackupTimes").ToString();

                backup_time[] backups = JsonConvert.DeserializeObject<backup_time[]>(JSONBackup);

                keyread.Close();

                return backups;
            }

            throw new Exception("Keyread was null");
        }

        public static void CreateBackupTargets(backup_target[] backups) //vloží backupy do registru //není omezená délka?
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user?

            string JSONBackup = JsonConvert.SerializeObject(backups);

            key.SetValue("BackupTargets", JSONBackup);
            key.Close();
        }
        public static backup_target[] GetBackupTargets() //bere backupy z registru
        {
            RegistryKey keyread = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user?

            if (keyread != null)
            {
                string JSONBackup = keyread.GetValue("BackupTargets").ToString();

                backup_target[] backups = JsonConvert.DeserializeObject<backup_target[]>(JSONBackup);

                keyread.Close();

                return backups;
            }

            throw new Exception("Keyread was null");
        }

        public static void CreateBackupSources(backup_source[] backups) //vloží backupy do registru //není omezená délka?
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user?

            string JSONBackup = JsonConvert.SerializeObject(backups);

            key.SetValue("BackupSources", JSONBackup);
            key.Close();
        }
        public static backup_source[] GetBackupSources() //bere backupy z registru
        {
            RegistryKey keyread = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user?

            if (keyread != null)
            {
                string JSONBackup = keyread.GetValue("BackupSources").ToString();

                backup_source[] backups = JsonConvert.DeserializeObject<backup_source[]>(JSONBackup);

                keyread.Close();

                return backups;
            }

            throw new Exception("Keyread was null");
        }
    }
}
