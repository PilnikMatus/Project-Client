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

                Console.WriteLine("Backupy nastavené na tento PC: nezobrazuje se");

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
            string id_client = HttpRequests.PostClient().id;

            RegistryUsing.CreateRegistryId(id_client);
        }
        public bool IsActive()
        {
            if (this.client.active)
                return true;
            else
                return false;
        }
    }
}
