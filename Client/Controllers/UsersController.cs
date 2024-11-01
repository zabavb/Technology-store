using Client.Models;
using Client.Models.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using Library.Models;
using Microsoft.AspNetCore.Authorization;

namespace Client.Controllers
{
    public class UsersController : Controller
    {
        private readonly string BaseAddress = "https://localhost:7074/";

        //============================ Logging ============================

        [HttpGet]
        public IActionResult Login(Status? status)
        {
            if (!string.IsNullOrEmpty(status!.Message))
                ViewBag.Status = status;

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = GetUserByUsernamePassword(model.Username, model.Password);

            if (user == null)
            {
                ViewBag.Status = new Status(false, "Incorrect username or password");
                return View(model);
            }

            CookieAuthenticationAsync(user.Result.Username, user.Result.Role!);

            return RedirectToAction("MyProfile", user);
        }

        [NonAction]
        private async Task<User> GetUserByUsernamePassword(string username, string password)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(BaseAddress);
                HttpResponseMessage response = await client.GetAsync($"gateway/users/{username}/{password}");

                return response.IsSuccessStatusCode ?
                    JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync())! :
                    new User();
            }
        }

        private async void CookieAuthenticationAsync(string username, string role)
        {
            using (HttpClient client = new())
            {
                List<Claim> claims = new()
                {
                    new(ClaimsIdentity.DefaultNameClaimType, username),
                    new(ClaimsIdentity.DefaultRoleClaimType, role),
                };

                var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            }
        }

        //============================ Logging out ============================

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", new Status(true, "Logout successful"));
        }

        //============================ MyProfile ============================

        [HttpGet]
        [Authorize]
        public IActionResult MyProfile(Status status, User user)
        {
            if (!string.IsNullOrEmpty(status.Message))
            {
                ViewBag.Status = status;
                return View(new User());
            }

            return View(user);
        }
    }
}
