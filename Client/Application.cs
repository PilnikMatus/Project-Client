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
        Client client = new Client();
        public void Run()
        {
            if(!IsInDBS())
            {
                Console.WriteLine("PC bude přidán do databáze");
                AddThisPC();
            }
            else
            {
                Console.WriteLine("PC je již v databázi");
            }

        }
        public bool IsInDBS()
        {
            Client[] clients = HttpRequests.GetRows();
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
            HttpRequests.Post();
        }
    }
}
