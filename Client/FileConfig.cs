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
            using (StreamWriter file = File.CreateText(@"C:\\BackupSW\client.txt"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, client);
            }
        }
        public static Client GetClient()
        {
            if (File.Exists(@"C:\\BackupSW\client.txt"))
            {
                using (StreamReader file = new StreamReader(@"C:\\BackupSW\client.txt"))
                {
                    return JsonConvert.DeserializeObject<Client>(file.ReadToEnd());
                }
            }
            else
                return null;
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
