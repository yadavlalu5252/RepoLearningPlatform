namespace RepoLearningPlatform.Interfaces
{
    public interface IRazorpayService
    {
        string CreateOrder(decimal amount);
        bool VerifyPayment(string paymentId, string orderId, string signature);
    }
}
