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

        public void Run()
        {            
            if(!IsInDBS())
            {
                Console.WriteLine("PC bude přidán do databáze");
                AddThisPC();
                Thread.Sleep(10000); //počkat na uložení - dělalo problémy
            }
            else
                Console.WriteLine("PC je v databázi");


            if (IsActive())
            {
                Console.WriteLine($"PC({this.client.name}): AKTIVNÍ");
                GetBackups();

                Console.WriteLine("Backupy nastavené na tento PC:");
                foreach (Backup item in GetBackups())
                {
                    if(item.active)
                        Console.WriteLine($"BACKUP -- Název: {item.name}, ID admina: {item.id_admin}, Backup typ: {item.backup_type}, Format typ: {item.format_type}, Poslední backup: {item.last_possible_backup_date}, Aktivní: {item.active}");
                }
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
            HttpRequests.PostClient();
        }
        public bool IsActive()
        {
            if (this.client.active)
                return true;
            else
                return false;
        }
        public List<int> GetIdBackup()
        {
            Job[] jobs = HttpRequests.GetJobRows();

            List<int> id_backup = new List<int>();

            foreach (var item in jobs)
            {
                if (this.client.id == item.id_client)
                {
                    id_backup.Add(item.id_backup);
                }
            }
            return id_backup;
            
        }
        public List<Backup> GetBackups()
        {
            List<int> id_backup = GetIdBackup(); //ID backupů

            Backup[] backups = HttpRequests.GetBackupRows(); //všechny backupy z tabulky BACKUP

            List<Backup> my_backups = new List<Backup>(); //BACKUPy souvisejici s tímto PC


            foreach (var item in backups)
            {
                foreach (int id in id_backup)
                {
                    if (id == item.id)
                    {
                        my_backups.Add(item);
                    }
                }
            }
            return my_backups;
        }
    }
}
