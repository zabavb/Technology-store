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
using System.Reflection;

namespace Client.Controllers
{
    public class AuthController : Controller
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
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await GetUserByUsernamePasswordAsync(model.Username, model.Password);

            if (user == null)
            {
                ViewBag.Status = new Status(false, "Incorrect username or password");
                return View(model);
            }

            await CookieAuthenticationAsync(user.Username, user.Role!);

            return View("MyProfile", user);
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
        public async Task<IActionResult> MyProfile(Status status, User user)
        {
            if (!string.IsNullOrEmpty(status.Message))
            {
                ViewBag.Status = status;
                return View(new User());
            }

            if (user.Id.Equals(0))
                user = await ControllersExtension.GetUserByUsernameAsync(User.Identity!.Name!, BaseAddress);
            if (user.Id.Equals(0))
                ViewBag.Status = new Status(false, $"Could not find the user {User.Identity!.Name!}");

            return View(user);
        }

        //============================ NonAction ============================

        [NonAction]
        private async Task<User> GetUserByUsernamePasswordAsync(string username, string password)
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

        [NonAction]
        private async Task CookieAuthenticationAsync(string username, string role)
        {
            List<Claim> claims = new()
            {
                new(ClaimsIdentity.DefaultNameClaimType, username),
                new(ClaimsIdentity.DefaultRoleClaimType, role),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14)
            };

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

    }
}
