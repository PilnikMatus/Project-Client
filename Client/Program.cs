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
            /*
            string host = "ftp://localhost:21";
            string UserId = "test";
            string Password = "123456";
            string path = "/didff";
            bool IsCreated = true;
            try
            {
                WebRequest request = WebRequest.Create(host + path);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(UserId, Password);
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine(resp.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
            */
            
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