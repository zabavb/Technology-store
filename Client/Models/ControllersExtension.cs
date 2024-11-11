using Library.Models;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;

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
                    return null!;
            }
        }

        public static async Task<List<Product>> GetProductsByIdsAsync(string baseAddress, long[]? longIds, string? stringIds)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(baseAddress);

                string idsQuery = string.Empty;

                if (longIds != null)
                    idsQuery = string.Join("&ids=", longIds);
                else if(stringIds != null)
                    idsQuery = stringIds.Replace(",", "&ids=");

                HttpResponseMessage response = await client.GetAsync($"gateway/products/many?ids={idsQuery}");

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<List<Product>>(await response.Content.ReadAsStringAsync())!;
                else
                    return null!;
            }
        }

        public static async Task<User> GetUserByUsernameAsync(string username, string baseAddress)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(baseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/users/username/{username}");

                return response.IsSuccessStatusCode ?
                    JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync())! :
                    null!;
            }
        }
    }
}
