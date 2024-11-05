using Client.Models;
using Client.Models.Orders;
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
        public async Task<IActionResult> GetBasket()
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

        //================================= Order =================================

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpGet]
        public async Task<IActionResult> GetOrder([FromQuery] long[] ids)
        {
            List<Product> products = new List<Product>();
            double sum = 0;
            foreach (var id in ids)
            {
                var product = await GetProductByIdAsync(id);

                if (product == null)
                {
                    ViewBag.Status = new Status(false, $"Could not find the product by id: {id}");
                    continue;
                }
                products.Add(product);
                sum += product.Price;
            }

            var user = await GetUserByUsernameAsync(User.Identity!.Name!);
            if (user == null) {
                ViewBag.Status = new Status(false, $"Could not find the user {User.Identity!.Name!}");
                return View("ProductList");
            }

            ViewBag.Sum = sum;
            OrderViewModel model = new OrderViewModel(products, user!);
            
            return View("Order", model);
        }

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpPost]
        public async Task<IActionResult> PostOrder(OrderViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Order", model);

            var user = await GetUserByUsernameAsync(User.Identity!.Name!);
            if (user == null)
            {
                ViewBag.Status = new Status(false, $"Could not find the user {User.Identity!.Name!}");
                return View("ProductList");
            }

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                var content = new StringContent(JsonConvert.SerializeObject(ModelExtension.ToOrder(model, user)), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"gateway/orders", content);

                if (response.IsSuccessStatusCode)
                    return View("ProductList", new Status(true, $"An order '{model.Id}' has been successfully made"));
                else
                    return View("ProductList", new Status(true, $"An occurred while processing an order '{model.Id}'"));
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

        [NonAction]
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/users/{username}");

                return response.IsSuccessStatusCode ?
                    JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync())! :
                    new User();
            }
        }
    }
}
