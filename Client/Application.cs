using ClientDemon.Tables;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientDemon
{
    public class Application
    {
        public Client client = new Client();
        public Backup backup = new Backup();

        public void Run()
        {            
            if(!IsInDBS())
            {
                Console.WriteLine("PC bude přidán do databáze");
                AddThisPC();
            }


            if (IsActive())
            {
                Console.WriteLine($"PC({this.client.name}): AKTIVNÍ");

                Console.WriteLine("Stahování backupů...");
                DownloadBackupDetails();
                Console.WriteLine("Backupy uloženy");

            }
            else
                Console.WriteLine($"PC({this.client.name}): NEAKTIVNÍ");

            
        }

        public bool IsInDBS()
        {
            Client client = HttpRequests.GetClient();

            if (client != null)
            {
                this.client = client;
                return true;
            }
            else
                return false;
            
        }
        public void AddThisPC()
        {
            Client client = HttpRequests.PostClient();

            RegistryUsing.CreateClient(client);
        }
        public bool IsActive()
        {
            if (this.client.active)
                return true;
            else
                return false;
        }
        private void DownloadBackupDetails()
        {
            RegistryUsing.CreateBackups(HttpRequests.PostGetBackups());
            RegistryUsing.CreateBackupTimes(HttpRequests.PostGetBackupTimes());
            RegistryUsing.CreateBackupTargets(HttpRequests.PostGetBackupTargets());
            RegistryUsing.CreateBackupSources(HttpRequests.PostGetBackupSources());
        }
    }
}
