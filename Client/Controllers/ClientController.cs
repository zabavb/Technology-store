using Client.Models;
using Client.Models.Orders;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using System.Reflection;
using System.Text;

namespace Client.Controllers
{
    public class ClientController : Controller
    {
        private readonly string BaseAddress = "https://localhost:7074/";

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
                    return View("Product/List", JsonConvert.DeserializeObject<IEnumerable<Product>>(await response.Content.ReadAsStringAsync()));
                else
                {
                    ViewBag.Status = new Status(false, "Could not load products");
                    return View("Product/List", new List<Product>());
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Product(Status status, long id)
        {
            if (!string.IsNullOrEmpty(status.Message))
                ViewBag.Status = status;

            var product = await ControllersExtension.GetProductByIdAsync(id, BaseAddress);

            if (product == null)
                ViewBag.Status = new Status(false, "Could not find the product");

            return View("Product/View", product);
        }

        //================================= Basket =================================

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpGet]
        public async Task<IActionResult> Basket(Status status)
        {
            if (!string.IsNullOrEmpty(status.Message))
                ViewBag.Status = status;

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/users/{User.Identity!.Name}/basket");

                if (response.IsSuccessStatusCode)
                {
                    var ids = JsonConvert.DeserializeObject<long[]>(await response.Content.ReadAsStringAsync())!;
                    
                    var products = await ControllersExtension.GetProductsByIdsAsync(BaseAddress, ids, null);
                    if (products == null)
                        ViewBag.Status = new Status(false, "Failed to load basket");
                    
                    StringBuilder str = new();
                    ids.ToList().ForEach(id => str.Append($"{id},"));
                    str.Remove(str.Length - 1, 1);

                    ViewBag.Ids = str.ToString();
                    ViewBag.Sum = CountSum(products!);
                    
                    return View("Basket/View", products);
                }
                else
                {
                    ViewBag.Status = new Status(false, "Failed to load basket");
                    return View("Basket/View", new List<Product>());
                }
            }
        }

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpGet]
        public async Task<IActionResult> PostBasket(long id)
        {
            var product = await ControllersExtension.GetProductByIdAsync(id, BaseAddress);

            if (product == null)
                return RedirectToAction("ProductList", new Status(false, "Could not find the product"));

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"gateway/users/{User.Identity!.Name}/basket", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ProductList", new Status(true, $"The product '{product.Name}' has been added to basket"));
                else
                    return RedirectToAction("ProductList", new Status(true, $"The product '{product.Name}' has NOT been successfully added to basket"));
            }
        }

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteBasket(long id)
        {
            if (await DeleteFromBasketAsync(id))
                return RedirectToAction("Basket", new Status(true, "Product has been successfully removed"));
                else
                return RedirectToAction("Basket", new Status(false, "Failed to remove product from the basket"));
            }

        //================================= Order =================================

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpGet]
        public async Task<IActionResult> Order(string ids, long id)
        {
            List<Product> products = new List<Product>();
            var list = ids.Split(",").ToList();
            
            if (ids.Length > 0)
            {
                foreach (var item in list)
            {
                var product = await ControllersExtension.GetProductByIdAsync(id, BaseAddress);

                if (product == null)
                {
                    ViewBag.Status = new Status(false, $"Could not find the product by id: {id}");
                    continue;
                }

                    products.Add(product);
                }
            }
            else
            {
                var product = await ControllersExtension.GetProductByIdAsync(id, BaseAddress);
                if (product == null)
                    return RedirectToAction("Basket", new Status(false, $"Could not find the product by id: {id}"));

                products.Add(product);
            }

            var user = await ControllersExtension.GetUserByUsernameAsync(User.Identity!.Name!, BaseAddress);
            if (user == null)
                return RedirectToAction("ProductList", new Status(false, $"Could not find the user {User.Identity!.Name!}"));

            ViewBag.Sum = CountSum(products);
            return View("Order/View", new OrderViewModel(products, user!));
        }

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpPost]
        public async Task<IActionResult> PostOrder(OrderViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Order/View", model);

            var user = await ControllersExtension.GetUserByUsernameAsync(User.Identity!.Name!, BaseAddress);
            if (user == null)
                return RedirectToAction("Basket", new Status(false, $"Could not find the user {User.Identity!.Name!}"));

            var products = await ControllersExtension.GetProductsByIdsAsync(BaseAddress, null, model.ItemsIds);
            if (products == null)
                return RedirectToAction("Basket", new Status(false, $"Could not find any product"));

            model.Items = products;

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                var content = new StringContent(JsonConvert.SerializeObject(ModelExtension.ToOrder(model, user)), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"gateway/orders", content);

                if (response.IsSuccessStatusCode)
                {
                    products.ForEach(p => DeleteFromBasketAsync(p.Id).Wait());

                    return RedirectToAction("ProductList", new Status(true, $"Order has been successfully made"));
                }
                else
                    return RedirectToAction("ProductList", new Status(true, $"An error occurred while processing an order"));
            }
        }

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.DeleteAsync($"gateway/orders/{id}");

                return RedirectToAction("Basket", response.IsSuccessStatusCode ?
                    new Status(true, "Order has been canceled") :
                    new Status(false, "Could not cancel the order"));
            }
        }

        //================================= NonAction =================================

        [NonAction]
        public string BuildStringIds(long[] ids)
        {
            if (ids.Length == 0)
                return string.Empty;

            List<long> list = ids.ToList();
            StringBuilder str = new StringBuilder();

            list.ForEach(id => str.Append($"ids={id}&"));
            list.Remove(list.Last());
            
            return list.ToString()!;
        }

        [NonAction]
        public double CountSum(List<Product> products)
        {
            double sum = 0;
            products.ForEach(p => sum += p.Price);
            double res = Math.Round(sum, 2);

            return res;
        }
            
        [NonAction]
        public async Task<bool> DeleteFromBasketAsync(long id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.DeleteAsync($"gateway/users/{User.Identity!.Name}/basket/{id}");

                return response.IsSuccessStatusCode;
            }
        }
    }
}
