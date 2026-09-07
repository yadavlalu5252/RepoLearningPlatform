using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using Razorpay.Api;
using RepoLearningPlatform.Data;
using RepoLearningPlatform.Interfaces;
using RepoLearningPlatform.Models;

namespace RepoLearningPlatform.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext db;
        public CartService(AppDbContext db)
        {
            this.db = db;
        }

        public void AddMasterCourse(int userId, int masterCourseId)
        {
            var course = db.MasterCourse.Find(masterCourseId);

            if (course != null)
            {
                var alreadyPurchased = db.Purchases
                    .Any(x => x.UserId == userId &&
                              x.MasterCourseId == masterCourseId);

                var alreadyInCart = db.Carts
                    .Any(x => x.UserId == userId &&
                              x.MasterCourseId == masterCourseId);

                if (!alreadyPurchased && !alreadyInCart)
                {
                    var amount = db.SubCourse
                        .Where(x => x.MasterCourseId == masterCourseId)
                        .Sum(x => x.Amount);

                    var cart = new Cart()
                    {
                        UserId = userId,
                        MasterCourseId = masterCourseId,
                        Amount = amount,
                        CreatedAt = DateTime.Now
                    };

                    db.Carts.Add(cart);
                    db.SaveChanges();
                }
                
            }
        }

        public void AddSubCourse(int userId, int subCourseId)
        {
            var course = db.SubCourse.Find(subCourseId);

            var alreadyPurchased = db.Purchases
            .Any(x => x.UserId == userId &&
                      x.SubCourseId == subCourseId);

            var alreadyInCart = db.Carts
                .Any(x => x.UserId == userId &&
                          x.SubCourseId == subCourseId);

            if (!alreadyPurchased && !alreadyInCart)
            {
                var cart = new Cart()
                {
                    UserId = userId,
                    SubCourseId = subCourseId,
                    Amount = course.Amount,
                    CreatedAt = DateTime.Now
                };

                db.Carts.Add(cart);
                db.SaveChanges();
            
        }
        }

        public void AddSubscription(int userId, int subscriptionId)
        {
            var subscription = db.Subscriptions.Find(subscriptionId);
            var alreadyPurchased = db.Purchases
            .Any(x => x.UserId == userId &&
                      x.SubscriptionId == subscriptionId);

            var alreadyInCart = db.Carts
                .Any(x => x.UserId == userId &&
                          x.SubscriptionId == subscriptionId);

            if (!alreadyPurchased && !alreadyInCart)
            {
                var cart = new Cart()
                {
                    UserId = userId,
                    SubscriptionId = subscriptionId,
                    Amount = subscription.amount,
                    CreatedAt = DateTime.Now
                };

                db.Carts.Add(cart);
                db.SaveChanges();
            
            
            }
        }

        public List<Cart> GetCartItems(int userId)
        {
            var data = db.Carts
        .Include(x => x.MasterCourseData)
        .Include(x => x.SubCourseData)
        .Include(x => x.SubscriptionData)
        .Where(x => x.UserId == userId)
        .ToList();
            return data;
        }

        public void RemoveFromCart(int cartId)
        {
            var cart = db.Carts.Find(cartId);
            if (cart != null)
            {
                db.Carts.Remove(cart);
                db.SaveChanges();
            }
        }
        public void SavePurchase(int userId, string paymentStatus)
        {
            var cartItems = db.Carts
                .Where(x => x.UserId == userId)
                .ToList();

            foreach (var item in cartItems)
            {
                var purchase = new Purchase()
                {
                    UserId = userId,
                    MasterCourseId = item.MasterCourseId,
                    SubCourseId = item.SubCourseId,
                    SubscriptionId = item.SubscriptionId,
                    Amount = item.Amount,
                    PaymentStatus = paymentStatus,
                    PurchaseDate = DateTime.Now,
                    Status = "Active"
                };

                db.Purchases.Add(purchase);
            }
            db.Carts.RemoveRange(cartItems);
            db.SaveChanges();
        }
    }
}
