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
using Microsoft.Win32;
using System.Net;

namespace ClientDemon
{
    public class Program
    {
        public static Application app = new Application();
        public static Backup bf = new Backup();
        static void Main(string[] args)
        {
            Timer timer = new Timer(Tick, null, 0, 20000);
            Timer timer2 = new Timer(BackupChecker, null, 0, 60000);

            while (true)
            {
                ConsoleKey info = Console.ReadKey().Key;

                if (info == ConsoleKey.F1)
                {
                    Backup bf = new Backup();
                    bf.CheckBackup();
                }
            }
        }
        public static void Tick(object O) //ve finale každou hodinu
        {
            Console.WriteLine("--- " + DateTime.Now + " ---");
            app.Run();
            Console.WriteLine();
        }
        public static void BackupChecker(object O) //každou minutu
        {

            //udělání zálohy
            Console.WriteLine("kontrolování záloh... (minuta)");
        }
    }
}