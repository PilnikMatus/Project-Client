using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Client
{
    #region snippet_prod
    public class Product
    {
        public string id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int phone { get; set; }
        public string log_repetation_unit { get; set; }
        public int log_repetation_number { get; set; }
        public int log_importance { get; set; }
    }
    #endregion

    class Program
    {

        static HttpClient client = new HttpClient();

        static void ShowProduct(Product product)
        {
            Console.WriteLine($"Name: {product.lastname}\tPrice: " +
                $"{product.firstname}\tCategory: {product.lastname}");
        }

        static async Task<Uri> CreateProductAsync(Product product)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/admin", product);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }



        static async Task<Product> GetProductAsync(string path)
        {
            Product product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<Product>();
            }
            return product;
        }

        static async Task<Product> UpdateProductAsync(Product product)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"api/admin/{product.id}", product);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            product = await response.Content.ReadAsAsync<Product>();
            return product;
        }

        static async Task<HttpStatusCode> DeleteProductAsync(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/admin/{id}");
            return response.StatusCode;
        }


        static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("http://localhost:49497/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Create a new product
                Product product = new Product
                {
                    firstname = "Test demona",
                    lastname = "521",
                    email = "Widgets",
                    password = "",
                    phone = 7498,
                    log_repetation_unit = "hour",
                    log_repetation_number = 2,
                    log_importance = 3
                };

                var url = await CreateProductAsync(product);
                Console.WriteLine($"Created at {url}");
                
                // Get the product
                product = await GetProductAsync(url.PathAndQuery);
                ShowProduct(product);
                
                // Update the product
                Console.WriteLine("Updating price...");
                product.phone = 80;
                await UpdateProductAsync(product);

                // Get the updated product
                product = await GetProductAsync(url.PathAndQuery);
                ShowProduct(product);
                
                // Delete the product
                var statusCode = await DeleteProductAsync(product.id);
                Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }

}