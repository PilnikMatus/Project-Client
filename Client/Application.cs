using ClientDemon.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon
{
    public class Application
    {
        public int id_client { get; set; }
        Client client = new Client();

        public void Run()
        {
            if(!IsInDBS())
            {
                Console.WriteLine("PC bude přidán do databáze");
                AddThisPC();
            }
            else
                Console.WriteLine("PC je již v databázi");

            if (IsActive())
            {
                Console.WriteLine("PC: AKTIVNÍ");
                GetBackups();

                Console.WriteLine("Backupy nastavené na tento PC:");
                foreach (Backup item in GetBackups())
                {
                    Console.WriteLine($"BACKUP -- Název: {item.name}, ID admina: {item.id_admin}, Backup typ: {item.backup_type}, Format typ: {item.format_type}, Poslední backup: {item.last_possible_backup_date}, Aktivní: {item.active}");
                }
            }
            else
                Console.WriteLine("PC: NEAKTIVNÍ");
            
        }

        public bool IsInDBS()
        {
            Client[] clients = HttpRequests.GetClientRows();
            foreach (var item in clients)
            {
                if (IPMethods.GetLocalIPAddress() == item.ip_address
                   && IPMethods.GetLocalMac() == item.mac_address)
                    return true;
            }
            return false;
        }
        public void AddThisPC()
        {
            HttpRequests.PostClient();
        }
        public bool IsActive()
        {
            Client[] clients = HttpRequests.GetClientRows();
            foreach (var item in clients)
            {
                if (IPMethods.GetLocalIPAddress() == item.ip_address
                   && IPMethods.GetLocalMac() == item.mac_address)
                {
                    this.id_client = Convert.ToInt32(item.id);
                    if (item.active)
                        return true;
                    else
                        return false;
                }
            }
            throw new Exception("Tento PC nebyl nalezen");
        }
        public List<int> GetIdBackup()
        {
            Job[] jobs = HttpRequests.GetJobRows();

            List<int> id_backup = new List<int>();

            foreach (var item in jobs)
            {
                if (this.id_client == item.id_client)
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
