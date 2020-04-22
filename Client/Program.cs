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
            //GET funguje
            
            //POST funguje

            //PUT
             using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:49497/api/");

                var student = new Client() { name = "Steve", mac_address = "ergreg", ip_address = "ergerg", active = 0 };

                var postTask = client.PutAsJsonAsync<Client>("client/3", student);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<Client>();
                    readTask.Wait();

                }
                else
                {
                    Console.WriteLine(result.StatusCode);
                }
            }
            //DELETE funguje

            Console.ReadLine();
        }
        
    }

}