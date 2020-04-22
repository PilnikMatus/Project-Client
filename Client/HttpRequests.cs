using ClientDemon.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemon
{
    public class HttpRequests
    {
        public void Get()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:49497/api/");

                //HTTP GET
                var responseTask = client.GetAsync("client");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsAsync<Client[]>();
                    readTask.Wait();

                    var students = readTask.Result;

                    foreach (var student in students)
                    {
                        Console.WriteLine(student.name);
                    }
                }
            }
        }
        public void Post()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:49497/api/");

                var student = new Client() { name = "Steve", mac_address = "", ip_address = "", active = 1 };

                var postTask = client.PostAsJsonAsync<Client>("client", student);
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
        }
        public void Put()
        {
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
        }
        public void Delete()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:49497/api/");

                int id_client = 1;
                var responseTask = client.DeleteAsync($"client/{id_client}");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    Console.WriteLine($"Client s id {id_client} byl smazán");

                }
            }
        }
    }
}
