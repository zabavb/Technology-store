using Client.Models;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace Client.Controllers
{
    public class ClientController : Controller
    {
        private readonly string BaseAddress = "https://localhost:7074/";

        [HttpGet]
        public IActionResult ManagePanel() => View();


        //================================= Product =================================

        [HttpGet]
        public async Task<IActionResult> ProductList(Status status)
        {
            if (!string.IsNullOrEmpty(status.Message))
                ViewBag.Status = status;

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync("gateway/products");

                if (response.IsSuccessStatusCode)
                    return View(JsonConvert.DeserializeObject<IEnumerable<Product>>(await response.Content.ReadAsStringAsync()));
                else
                {
                    ViewBag.Status = new Status(false, "An issue occurred during the navigation to the product");
                    return RedirectToAction("ProductList");
                }
            }
        }

        [HttpGet]
        public IActionResult Product(Status status, long id)
        {
            if (!string.IsNullOrEmpty(status.Message))
                ViewBag.Status = status;

            var product = GetProductByIdAsync(id);
            
            if (product == null)
                ViewBag.Status = new Status(false, "Could not find the product");

            return View(product!.Result);
        }

        //================================= Basket =================================

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpGet]
        public async Task<IActionResult> BasketList()
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/users/{User.Identity!.Name}/basket");

                if (response.IsSuccessStatusCode)
                    return View(JsonConvert.DeserializeObject<IEnumerable<Product>>(await response.Content.ReadAsStringAsync())!);
                else
                    return View(new List<Product>());
            }
        }

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpGet]
        public async Task<IActionResult> PostToBasket(long id)
        {
            var product = GetProductByIdAsync(id);

            if (product == null)
                ViewBag.Status = new Status(false, "Could not find the product");

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                var content = new StringContent(JsonConvert.SerializeObject(product!.Result), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"gateway/users/{User.Identity!.Name}/basket", content);

                if (response.IsSuccessStatusCode)
                    return View("ProductList", new Status(true, $"The product '{product.Result.Name}' has been added to basket"));
                else
                    return View("ProductList", new Status(true, $"The product '{product.Result.Name}' has NOT been successfully added to basket"));
            }
        }

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteFromBasket(long id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.DeleteAsync($"gateway/users/{User.Identity!.Name}/basket/{id}");

                if (response.IsSuccessStatusCode)
                    return View("ProductList", new Status(true, "Product has been successfully removed"));
                    return View("ProductList", new Status(false, "Failed to remove product from the basket"));
            }
        }

        //================================= Purchase =================================

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpGet]
        public async Task<IActionResult> PurchaseList()
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/users/{User.Identity!.Name}/purchase");

                if (response.IsSuccessStatusCode)
                    return View(JsonConvert.DeserializeObject<IEnumerable<Product>>(await response.Content.ReadAsStringAsync())!);
                else
                    return View(new List<Product>());
            }
        }


        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpGet]
        public async Task<IActionResult> Purchase(long id)
        {
            var product = GetProductByIdAsync(id);

            if (product == null)
                ViewBag.Status = new Status(false, "Could not find the product");

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                var content = new StringContent(JsonConvert.SerializeObject(product!.Result), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"gateway/users/{User.Identity!.Name}/purchase", content);

                if (response.IsSuccessStatusCode)
                    return View("ProductList", new Status(true, $"The product '{product.Result.Name}' has been purchased"));
                else
                    return View("ProductList", new Status(true, $"The product '{product.Result.Name}' has NOT been successfully purchased"));
            }
        }

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteFromPurchases(long id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.DeleteAsync($"gateway/users/{User.Identity!.Name}/purchase/{id}");

                if (response.IsSuccessStatusCode)
                    return View("ProductList", new Status(true, "Product has been successfully removed"));
                return View("ProductList", new Status(false, "Failed to remove product from the purchases"));
            }
        }

        //================================= NonAction =================================

        [NonAction]
        public async Task<Product> GetProductByIdAsync(long id)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/products/{id}");

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Product>(await response.Content.ReadAsStringAsync())!;
                else
                    return new Product();
            }
        }
    }
}
