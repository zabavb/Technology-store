using Client.Models;
using Client.Models.Products;
using Client.Models.Users;
using Library.Infrastructure;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Client.Controllers
{
    [Authorize(Roles = "Moderator, Admin")]
    public class ManageController : Controller
    {
        private readonly string BaseAddress = "https://localhost:7074/";

        [HttpGet]
        public IActionResult ManagePanel() => View();

        //================================= User =================================

        [HttpGet]
        public async Task<IActionResult> UserList(Status status)
        {
            if (!string.IsNullOrEmpty(status.Message))
                ViewBag.Status = status;

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync("gateway/users");

                if (response.IsSuccessStatusCode)
                    return View(JsonConvert.DeserializeObject<IEnumerable<User>>(await response.Content.ReadAsStringAsync()));
                else
                {
                    ViewBag.Status = new Status(false, "An issue occurred during the navigation to the user");
                    return RedirectToAction("UserList");
                }
            }
        }

        // May occurre an error, solution - rename action
        [HttpGet]
        public IActionResult User(Status status, long id)
        {
            if (!string.IsNullOrEmpty(status.Message))
                ViewBag.Status = status;

            var user = GetUserByIdAsync(id);

            if (user == null)
                ViewBag.Status = new Status(false, "Could not find the user");

            return View(user!.Result);
        }

        [HttpGet]
        public IActionResult PostUser()
        {
            ViewBag.IsUser = true;
            ViewBag.IsPost = true;
            return View("Manage", new ManageUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> PostUser(ManageUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsUser = true;
                ViewBag.IsPost = true;
                return View("Manage", model);
            }

            model.Password = UserExtension.HashPassword(model.Password);

            using (HttpClient client = new())
            {
                var content = new StringContent(JsonConvert.SerializeObject(ModelExtension.ToUser(model)), Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.PostAsync("gateway/users", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("UserList", new Status(true, $"The user {model.Username} has been successfully registered."));
                else
                {
                    ViewBag.IsUser = true;
                    ViewBag.IsPost = true;
                    return View("Manage");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> PutUser(long id)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/users/{id}");

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.IsUser = true;
                    ViewBag.IsPost = false;
                    var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
                    return View("Manage", new ManageUserViewModel(user!));
                }
                else
                    return View("UserList", new Status(false, "User data is missing"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> PutUser(ManageUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsUser = true;
                ViewBag.IsPost = false;
                return View("Manage", model);
            }

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync($"gateway/users/{model.Id}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("UserList", new Status(true, $"The user {model.Username} has been successfully updated"));
                else
                {
                    ViewBag.IsUser = true;
                    ViewBag.IsPost = false;
                    return View("Manage", model);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(long id)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.DeleteAsync($"gateway/users/{id}");

                return RedirectToAction("UserList", response.IsSuccessStatusCode ?
                    ViewBag.Status = new Status(true, "User has been successfully deleted") :
                    ViewBag.Status = new Status(false, "An issue occurred during the deletion of the user"));
            }
        }

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

        [HttpGet]
        public IActionResult PostProduct()
        {
            ViewBag.IsUser = false;
            ViewBag.IsPost = true;
            return View("Manage", new ManageProductViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct(ManageProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsUser = false;
                ViewBag.IsPost = true;
                return View("Manage", model);
            }

            using (HttpClient client = new())
            {
                var content = new StringContent(JsonConvert.SerializeObject(ModelExtension.ToProduct(model)), Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.PostAsync("gateway/products", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ProductList", new Status(true, $"The product {model.Brand} {model.Model} has been successfully added"));
                else
                {
                    ViewBag.IsUser = false;
                    ViewBag.IsPost = true;
                    return View("Manage");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Put(long id)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/products/{id}");

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.IsUser = false;
                    ViewBag.IsPost = false;
                    var product = JsonConvert.DeserializeObject<Product>(await response.Content.ReadAsStringAsync());
                    return View("Manage", new ManageProductViewModel(product!));
                }
                else
                    return View("ProductList", new Status(false, "Product data is missing"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Put(ManageProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsUser = false;
                ViewBag.IsPost = false;
                return View("Manage", model);
            }

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync($"gateway/products/{model.Id}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ProductList", new Status(true, $"The product {model.Brand} {model.Model} has been successfully updated"));
                else
                {
                    ViewBag.IsUser = false;
                    ViewBag.IsPost = false;
                    return View("Manage", model);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.DeleteAsync($"gateway/products/{id}");

                return RedirectToAction("ProductList", response.IsSuccessStatusCode ?
                    ViewBag.Status = new Status(true, "Product has been successfully deleted") :
                    ViewBag.Status = new Status(false, "An issue occurred during the deletion of the product"));
            }
        }

        //================================= NonAction =================================

        [NonAction]
        public async Task<User> GetUserByIdAsync(long id)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/users/{id}");

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync())!;
                else
                    return new User();
            }
        }

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
