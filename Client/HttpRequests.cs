using ClientDemon.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon
{
    public static class HttpRequests
    {
        public static string URL { get; set; } = "http://192.168.1.198/api/";

        public static Client[] GetClientRows()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);

                var responseTask = client.GetAsync("client");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<Client[]>();
                    readTask.Wait();

                    var Clients = readTask.Result;

                    return Clients;
                }
                throw new Exception("Tento PC nebyl nalezen");
            }
        }
        public static void PostClient()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                

                var Client = new Client() { name = IPMethods.GetLocalPCName(), mac_address = IPMethods.GetLocalMac(), ip_address = IPMethods.GetLocalIPAddress() };

                var postTask = client.PostAsJsonAsync<Client>("client", Client);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<Client>();
                    readTask.Wait();

                    Console.WriteLine("Údaje o PC odeslány");
                }
                else
                {
                    Console.WriteLine(result.StatusCode);
                }
            }
        }
        public static Backup[] GetBackupRows()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);

                var responseTask = client.GetAsync("backup");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<Backup[]>();
                    readTask.Wait();

                    var Backups = readTask.Result;

                    return Backups;
                }
                throw new Exception("Tento PC nebyl nalezen");
            }
        }
        public static Job[] GetJobRows()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);

                var responseTask = client.GetAsync("job");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<Job[]>();
                    readTask.Wait();

                    var Jobs = readTask.Result;

                    return Jobs;
                }
                throw new Exception("Tento PC nebyl nalezen");
            }
        }
        //PUT a DELETE nebude vlastně vůbec potřeba? Ip a Mac se aktualizovat nebude, nebyla by možnost, jak ověřit, že je to ten samej PC?

        /*       public static void Put()
               {
                   using (var client = new HttpClient())
                   {
                       client.BaseAddress = new Uri(URL);

                       var Client = new Client() { name = "Steve", mac_address = "ergreg", ip_address = "ergerg", active = false };

                       var postTask = client.PutAsJsonAsync<Client>("client/3", Client);
                       postTask.Wait();

                       var result = postTask.Result;
                       if (result.IsSuccessStatusCode)
                       {

                           var readTask = result.Content.ReadAsAsync<Client>();
                           readTask.Wait();

                           Console.WriteLine("Údaje odeslány");
                       }
                       else
                       {
                           Console.WriteLine(result.StatusCode);
                       }
                   }
               }
               public static void Delete()
               {
                   using (var client = new HttpClient())
                   {
                       client.BaseAddress = new Uri(URL);

                       int id_client = 1;
                       var responseTask = client.DeleteAsync($"client/{id_client}");
                       responseTask.Wait();

                       var result = responseTask.Result;
                       if (result.IsSuccessStatusCode)
                           Console.WriteLine($"Client s id {id_client} byl smazán");
                   }
               }*/

        public static void Get()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);

                var responseTask = client.GetAsync("client");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<Client[]>();
                    readTask.Wait();

                    var Clients = readTask.Result;

                    foreach (var item in Clients)
                    {
                        Console.WriteLine(item.name);
                    }
                }
            }
        }
    }
}
