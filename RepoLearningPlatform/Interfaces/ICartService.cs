using RepoLearningPlatform.Models;

namespace RepoLearningPlatform.Interfaces
{
    public interface ICartService
    {
        void AddMasterCourse(int userId, int masterCourseId);
        void AddSubCourse(int userId, int subCourseId);
        void AddSubscription(int userId, int subscriptionId);

        List<Cart> GetCartItems(int userId);

        void RemoveFromCart(int cartId);
        void SavePurchase(int userId, string paymentStatus);
    }
}
