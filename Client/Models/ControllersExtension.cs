using Library.Models;
using Newtonsoft.Json;

namespace Client.Models
{
    public static class ControllersExtension
    {
        public static async Task<Product> GetProductByIdAsync(long id, string baseAddress)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(baseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/products/{id}");

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Product>(await response.Content.ReadAsStringAsync())!;
                else
                    return new Product();
            }
        }

        public static async Task<List<Product>> GetProductsByIdsAsync(long[] ids, string baseAddress)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(baseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/products/{ids}");

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<List<Product>>(await response.Content.ReadAsStringAsync())!;
                else
                    return new List<Product>();
            }
        }

        public static async Task<User> GetUserByUsernameAsync(string username, string baseAddress)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(baseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/users/{username}");

                return response.IsSuccessStatusCode ?
                    JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync())! :
                    new User();
            }
        }
    }
}
