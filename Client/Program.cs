using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using ClientDemon.Tables;

namespace ClientDemon
{
class Program
 {
        static void Main(string[] args)
        {
            HttpRequests httpRequests = new HttpRequests();

            httpRequests.Post();

            Console.ReadLine();
        }
        
    }

}