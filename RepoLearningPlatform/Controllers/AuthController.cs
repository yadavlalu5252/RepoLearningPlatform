using Microsoft.AspNetCore.Mvc;
using RepoLearningPlatform.Interfaces;
using RepoLearningPlatform.Models;
using RepoLearningPlatform.Services;

namespace RepoLearningPlatform.Controllers
{
    public class AuthController : Controller
    {
        IAuthService ias;
        public AuthController(IAuthService ias)
        {
            this.ias = ias;
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (user == null ||
                string.IsNullOrEmpty(user.Name) ||
                string.IsNullOrEmpty(user.Username) ||
                string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(user.Password))
            {
                ViewBag.ErrorMessage = "All Fields are mandatory!";
                return View(user);
            }

            var result = ias.RegisterUser(user);

            if (result)
            {
                TempData["SuccessMessage"] = "Registration Successful! Please login.";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.ErrorMessage = "User with this email already exists.";
                return View(user);
            }
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            if (user == null ||
                string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(user.Password))
            {
                ViewBag.ErrorMessage = "Email and Password are mandatory!";
                return View(user);
            }

            var data = ias.LoginUser(user);

            if (data != null)
            {
                HttpContext.Session.SetInt32("UserId", data.UserId);
                HttpContext.Session.SetString("Username", data.Username);

                TempData["SuccessMessage"] = "Login Successful!";

                return RedirectToAction("Index", "UserDashboard");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid Email or Password!";
                return View(user);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}
