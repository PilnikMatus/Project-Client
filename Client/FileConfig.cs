using ClientDemon.Tables;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon
{
    public static class FileConfig
    {
        public static void CreateClient(Client client)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user?

            string JSONClient = JsonConvert.SerializeObject(client);
            key.SetValue("Client", JSONClient);
            key.Close();
        }
        public static Client GetClient()
        {
            RegistryKey keyread = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Backup-Client"); //pozor - pouze current user?
            Client client = null;

            if (keyread != null)
            {
                if (keyread.GetValue("Client") != null)
                {
                    string JSONBackup = keyread.GetValue("Client").ToString();
                    client = JsonConvert.DeserializeObject<Client>(JSONBackup);
                }
                keyread.Close();
            }
            return client;
        }
        public static void CreateBackups(fullBackupInfo[] backups)
        {
            using (StreamWriter file = File.CreateText(@"C:\\BackupSW\config.txt"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, backups);
            }
        }
        public static fullBackupInfo[] GetBackups() //bere backupy z registru
        {
            using (StreamReader file = new StreamReader(@"C:\\BackupSW\config.txt"))
            {
                return JsonConvert.DeserializeObject<fullBackupInfo[]>(file.ReadToEnd());
            }
        }
    }
}
