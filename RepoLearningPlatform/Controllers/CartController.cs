using RepoLearningPlatform.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RepoLearningPlatform.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService cs;
        private readonly IRazorpayService rs;
        private readonly IConfiguration configuration;

        public CartController(ICartService cs, IRazorpayService rs, IConfiguration configuration)
        {
            this.cs = cs;
            this.rs = rs;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var cartItems = cs.GetCartItems(userId);

            return View(cartItems);
        }

        public IActionResult Checkout()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var cartItems = cs.GetCartItems(userId);

            ViewBag.RazorpayKey = configuration["Razorpay:KeyId"];

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult CreateOrder()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var cartItems = cs.GetCartItems(userId);

            decimal total = cartItems.Sum(x => x.Amount);

            string orderId = rs.CreateOrder(total);

            return Json(new
            {
                orderId = orderId,
                amount = total * 100
            });
        }

        [HttpPost]
        public IActionResult VerifyPayment(
    string paymentId,
    string orderId,
    string signature)
        {
            try
            {
                bool result = rs.VerifyPayment(
                    paymentId,
                    orderId,
                    signature);

                if (result)
                {
                    int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

                    cs.SavePurchase(userId, "Success");

                    return Json(new
                    {
                        success = true,
                        message = "Payment verified successfully"
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Payment verification failed"
                });
            }
            catch
            {
                return Json(new
                {
                    success = false,
                    message = "Payment verification failed"
                });
            }
        }

        [HttpPost]
        public IActionResult AddMasterCourse(int id)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            cs.AddMasterCourse(userId, id);

            return RedirectToAction("Index", "AllCourses");
        }

        [HttpPost]
        public IActionResult AddSubCourse(int id)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            cs.AddSubCourse(userId, id);

            return RedirectToAction("Index", "AllCourses");
        }

        [HttpPost]
        public IActionResult AddSubscription(int id)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            cs.AddSubscription(userId, id);

            return RedirectToAction("Index", "AllCourses");
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            cs.RemoveFromCart(id);

            return RedirectToAction("Index");
        }
    }
}