using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using ClientDemon.Tables;
using System.Threading;
using System.Runtime.CompilerServices;

namespace ClientDemon
{
    public class Program
    {
        public static Application app = new Application();
        static void Main(string[] args)
        {
            Timer timer = new Timer(Tick, null, 0, 5000);

            while (true)
            {
                ConsoleKey info = Console.ReadKey().Key;

                if (info == ConsoleKey.Escape)
                    Environment.Exit(0);
            }
        }
        public static void Tick(object O)
        {
            Console.WriteLine("---" + DateTime.Now + "---");
            app.Run();
            Console.WriteLine();
        }
    }
}