using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using ClientDemon.Tables;
using System.Threading;

namespace ClientDemon
{
    class Program
    {
        static void Main(string[] args)
        {

            Timer timer = new Timer(Tick, null, 0, 5000);

            while (true)
            {
                //check jestli už je tento v PC v databazi, pokud ano, dále; pokud ne, přidá se.
                //check jestli je aktivní,
                //pokud ano, zjistí, data o zálohování
            }
        }
        public static void Tick(object O)
        {
            Console.WriteLine("Update");

            HttpRequests httpRequests = new HttpRequests();

            httpRequests.Post();
        }

    }

}