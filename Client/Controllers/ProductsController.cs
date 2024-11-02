using Client.Models;
using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class ProductsController : Controller
    {
        private readonly string BaseAddress = "https://localhost:7074/";

        [HttpGet]
        public async Task<IActionResult> ProductsList(Status status)
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
                    return RedirectToAction("ProductsList");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Product(Status status, long id)
        {
            if (!string.IsNullOrEmpty(status.Message))
                ViewBag.Status = status;

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/products/{id}");

                if (response.IsSuccessStatusCode)
                    return View(JsonConvert.DeserializeObject<Product>(await response.Content.ReadAsStringAsync())!);
                else
                    return View(new Product());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Basket()
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/users/{User.Identity!.Name}");

                if (response.IsSuccessStatusCode)
                    return View(JsonConvert.DeserializeObject<IEnumerable<Product>>(await response.Content.ReadAsStringAsync())!);
                else
                    return View(new List<Product>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddToBasket(long id)
        {
            // Add to product to basket by id
        }

        [HttpGet]
        public async Task<IActionResult> Order(long id)
        {
            // Show details of purchase
        }

        [HttpGet]
        public async Task<IActionResult> Ordering(long id)
        {
            // Make a purchase of product by id
        }
    }
}
