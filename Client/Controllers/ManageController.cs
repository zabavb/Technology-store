﻿using Client.Models;
using Client.Models.Orders;
using Client.Models.Products;
using Client.Models.Users;
using Library.Infrastructure;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using static NuGet.Packaging.PackagingConstants;

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
        public async Task<IActionResult> UserList(Status status, string search)
        {
            ViewBag.Search = search;
            if (!string.IsNullOrEmpty(status.Message))
                ViewBag.Status = status;

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);

                if (!string.IsNullOrEmpty(search))
                {
                    HttpResponseMessage response = await client.GetAsync($"gateway/users/search/{search}");
                    if (response.IsSuccessStatusCode)
                        return View("User/List", JsonConvert.DeserializeObject<IEnumerable<User>>(await response.Content.ReadAsStringAsync()));
                    else
                    {
                        ViewBag.Status = new Status(false, "An error occurred, search is currently unreachable");
                        return View("User/List", new List<User>());
                    }
                }
                else
                {
                    HttpResponseMessage response = await client.GetAsync("gateway/users");

                    if (response.IsSuccessStatusCode)
                        return View("User/List", JsonConvert.DeserializeObject<IEnumerable<User>>(await response.Content.ReadAsStringAsync()));
                    else
                        return View("User/List", new Status(false, "Empty user list"));
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> User(Status status, long id)
        {
            if (!string.IsNullOrEmpty(status.Message))
                ViewBag.Status = status;

            var user = await ControllersExtension.GetUserByIdAsync(id, BaseAddress);

            if (user == null)
                ViewBag.Status = new Status(false, "Could not find the user");
            else
            {
                if (user.BasketIds.Count() > 0)
                    user.Basket = await ControllersExtension.GetProductsByIdsAsync(BaseAddress, user.BasketIds.ToArray(), null);
            }

            return View("User/View", user);
        }

        [HttpGet]
        public IActionResult PostUser()
        {
            ViewBag.IsPost = true;
            return View("User/Manage", new ManageUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> PostUser(ManageUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsPost = true;
                return View("User/Manage", model);
            }

            model.Password = UserExtension.HashPassword(model.Password);

            using (HttpClient client = new())
            {
                var content = new StringContent(JsonConvert.SerializeObject(ModelExtension.ToUser(model)), Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.PostAsync("gateway/users", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("UserList", new Status(true, $"The user {model.Username} has been successfully registered"));
                else
                {
                    ViewBag.IsPost = true;
                    return View("User/Manage");
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
                    ViewBag.IsPost = false;
                    var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
                    if (user == null)
                        return RedirectToAction("UserList", new Status(false, $"Could not find the user"));
                    else
                        user.Basket = await ControllersExtension.GetProductsByIdsAsync(BaseAddress, user.BasketIds.ToArray(), null);

                    return View("User/Manage", new ManageUserViewModel(user));
                }
                else
                    return RedirectToAction("UserList", new Status(false, $"User data is missing"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> PutUser(ManageUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsPost = false;
                return View("User/Manage", model);
            }

            model.Password = UserExtension.HashPassword(model.Password);

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                StringContent content = new StringContent(JsonConvert.SerializeObject(ModelExtension.ToUser(model)), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync($"gateway/users/{model.Id}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("UserList", new Status(true, $"The user {model.Username} has been successfully updated"));
                else
                {
                    ViewBag.IsPost = false;
                    ViewBag.Status = new Status(false, $"Failed to update the user {model.Username}");
                    return View("User/Manage", model);
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
                    new Status(true, "User has been successfully deleted") :
                    new Status(false, "An issue occurred during the deletion of the user"));
            }
        }

        //================================= Product =================================
        
        [HttpGet]
        public async Task<IActionResult> ProductList(Status status, string search)
        {
            ViewBag.Search = search;
            if (!string.IsNullOrEmpty(status.Message))
                ViewBag.Status = status;

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                
                if (!string.IsNullOrEmpty(search))
                {
                    HttpResponseMessage response = await client.GetAsync($"gateway/products/search/{search}");
                    if (response.IsSuccessStatusCode)
                        return View("Product/List", JsonConvert.DeserializeObject<IEnumerable<Product>>(await response.Content.ReadAsStringAsync()));
                    else
                    {
                        ViewBag.Status = new Status(false, "An error occurred, search is currently unreachable");
                        return View("Product/List", new List<Product>());
                    }
                }
                else
                {
                    HttpResponseMessage response = await client.GetAsync("gateway/products");

                    if (response.IsSuccessStatusCode)
                        return View("Product/List", JsonConvert.DeserializeObject<IEnumerable<Product>>(await response.Content.ReadAsStringAsync()));
                    else
                    {
                        ViewBag.Status = new Status(false, "Could not load products");
                        return View("Product/List", new Status(false, "Empty product list"));
                    }
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

        [HttpGet]
        public IActionResult PostProduct()
        {
            ViewBag.IsPost = true;
            return View("Product/Manage", new ManageProductViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct(ManageProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsPost = true;
                return View("Product/Manage", viewModel);
            }

            using (HttpClient client = new())
            {
                var content = new StringContent(JsonConvert.SerializeObject(ModelExtension.ToProduct(viewModel)), Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.PostAsync("gateway/products", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ProductList", new Status(true, $"The product {viewModel.Brand} {viewModel.Model} has been successfully added"));
                else
                {
                    ViewBag.IsPost = true;
                    return View("Product/Manage");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> PutProduct(long id)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/products/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var product = JsonConvert.DeserializeObject<Product>(await response.Content.ReadAsStringAsync());
                    if (product == null)
                        return RedirectToAction("ProductList", new Status(false, "Could not find the product"));

                    ViewBag.IsPost = false;
                    return View("Product/Manage", new ManageProductViewModel(product));
                }
                else
                    return RedirectToAction("ProductList", new Status(false, "Product data is missing"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> PutProduct(ManageProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsPost = false;
                return View("Product/Manage", viewModel);
            }

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                StringContent content = new StringContent(JsonConvert.SerializeObject(ModelExtension.ToProduct(viewModel)), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync($"gateway/products/{viewModel.Id}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ProductList", new Status(true, $"The product {viewModel.Brand} {viewModel.Model} has been successfully updated"));
                else
                {
                    ViewBag.IsPost = false;
                    ViewBag.Status = new Status(false, $"Failed to update the product {viewModel.Brand} {viewModel.Model}");
                    return View("Product/Manage", viewModel);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.DeleteAsync($"gateway/products/{id}");

                return RedirectToAction("ProductList", response.IsSuccessStatusCode ?
                    new Status(true, "Product has been successfully deleted") :
                    new Status(false, "An issue occurred during the deletion of the product"));
            }
        }

        //================================= Order =================================

        [HttpGet]
        public async Task<IActionResult> OrderList(Status status, string search)
        {
            ViewBag.Search = search;
            if (!string.IsNullOrEmpty(status.Message))
                ViewBag.Status = status;

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                if (!string.IsNullOrEmpty(search))
                {
                    HttpResponseMessage response = await client.GetAsync($"gateway/orders/{search}");
                    if (response.IsSuccessStatusCode)
                        return View("Order/List", new List<Order>() { JsonConvert.DeserializeObject<Order>(await response.Content.ReadAsStringAsync())! });
                    else
                        return View("Order/List", new List<Order>());
                }
                else
                {
                    HttpResponseMessage response = await client.GetAsync("gateway/orders");

                    if (response.IsSuccessStatusCode)
                    {
                        List<Order> orders = (List<Order>)JsonConvert.DeserializeObject<IEnumerable<Order>>(await response.Content.ReadAsStringAsync())!;

                        foreach (var order in orders)
                        {
                            order.Items = await ControllersExtension.GetProductsByIdsAsync(BaseAddress, order.ItemsIds.ToArray(), null);
                            order.Receiver = await ControllersExtension.GetUserByIdAsync(order.ReceiverId, BaseAddress);
                        }

                        return View("Order/List", orders);
                    }
                    else
                        return View("ManagePanel", new Status(false, "Empty order list"));
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Order(Status status, long id)
        {
            if (!string.IsNullOrEmpty(status.Message))
                ViewBag.Status = status;

            var order = await GetOrderByIdAsync(id);

            order.Items = await ControllersExtension.GetProductsByIdsAsync(BaseAddress, order.ItemsIds.ToArray(), null);
            order.Receiver = await ControllersExtension.GetUserByIdAsync(order.ReceiverId, BaseAddress);

            if (order == null)
                ViewBag.Status = new Status(false, "Could not find the order");

            return View("Order/View", order);
        }

        [HttpGet]
        public IActionResult PostOrder()
        {
            ViewBag.IsPost = true;
            return View("Order/Manage", new OrderViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> PostOrder(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsPost = true;
                return View("Order/Manage", model);
            }

            var user = await ControllersExtension.GetUserByUsernameAsync(model.ReceiverUsername, BaseAddress);
            if (user == null)
            {
                ViewBag.IsPost = true;
                ViewBag.Status = new Status(false, "Could not find the user");
                return View("Order/Manage", model);
            }

            var items = await ControllersExtension.GetProductsByIdsAsync(BaseAddress, null, model.ItemsIds);
            if (items == null)
            {
                ViewBag.IsPost = true;
                ViewBag.Status = new Status(false, $"Could not find any product by ids: {model.ItemsIds.ToString()}");
                return View("Order/Manage", model);
            }

            model.Items = items;
            
            using (HttpClient client = new())
            {
                var content = new StringContent(JsonConvert.SerializeObject(ModelExtension.ToOrder(model, user)), Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.PostAsync("gateway/orders", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("OrderList", new Status(true, $"Order for {model.ReceiverUsername} has been successfully made"));
                else
                {
                    ViewBag.IsPost = true;
                    return View("Order/Manage");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> PutOrder(long id)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/orders/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var order = JsonConvert.DeserializeObject<Order>(await response.Content.ReadAsStringAsync());
                    if (order == null)
                        return RedirectToAction("OrderList", new Status(false, "Could not find the order"));

                    var items = await ControllersExtension.GetProductsByIdsAsync(BaseAddress, order.ItemsIds.ToArray(), null);
                    if (items == null)
                        return RedirectToAction("OrderList", new Status(false, "Could not load order's items"));

                    var receiver = await ControllersExtension.GetUserByIdAsync(order.ReceiverId, BaseAddress);
                    if (receiver == null)
                        return RedirectToAction("OrderList", new Status(false, "Could not find the user"));

                    ViewBag.IsPost = false;
                    return View("Order/Manage", new OrderViewModel(order, items, receiver));
                }
                else
                    return RedirectToAction("OrderList", new Status(false, "Order data is missing"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> PutOrder(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsPost = false;
                return View("Order/Manage", model);
            }

            var user = await ControllersExtension.GetUserByUsernameAsync(model.ReceiverUsername, BaseAddress);
            if (user == null)
            {
                ViewBag.IsPost = false;
                ViewBag.Status = new Status(false, "Could not find the user");
                return View("Order/Manage", model);
            }

            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                StringContent content = new StringContent(JsonConvert.SerializeObject(ModelExtension.ToOrder(model, user)), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync($"gateway/orders/{model.Id}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("OrderList", new Status(true, $"The order {model.Id} has been successfully updated"));
                else
                {
                    ViewBag.IsPost = false;
                    ViewBag.Status = new Status(false, $"Failed to update the order {model.Id}");
                    return View("Order/Manage", model);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.DeleteAsync($"gateway/orders/{id}");

                return RedirectToAction("OrderList", response.IsSuccessStatusCode ?
                    new Status(true, "Order has been successfully deleted") :
                    new Status(false, "An issue occurred during the deletion of the order"));
            }
        }

        //================================= NonAction =================================

        [NonAction]
        public async Task<Order> GetOrderByIdAsync(long id)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/orders/{id}");

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<Order>(await response.Content.ReadAsStringAsync())!;
                else
                    return null!;
            }
        }
    }
}
