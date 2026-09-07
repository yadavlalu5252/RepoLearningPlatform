using Razorpay.Api;
using RepoLearningPlatform.Interfaces;

namespace RepoLearningPlatform.Services
{
    public class RazorpayService : IRazorpayService
    {
        private readonly IConfiguration configuration;

        public RazorpayService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CreateOrder(decimal amount)
        {
            string key = configuration["Razorpay:KeyId"];
            string secret = configuration["Razorpay:KeySecret"];

            RazorpayClient client = new RazorpayClient(key, secret);

            Dictionary<string, object> options = new Dictionary<string, object>();

            options.Add("amount", Convert.ToInt32(amount * 100));
            options.Add("currency", "INR");
            options.Add("receipt", "receipt_" + DateTime.Now.Ticks);

            Order order = client.Order.Create(options);

            string orderId = order["id"].ToString();

            return orderId;
        }

        public bool VerifyPayment(
            string paymentId,
            string orderId,
            string signature)
        {
            string key = configuration["Razorpay:KeyId"];
            string secret = configuration["Razorpay:KeySecret"];

            RazorpayClient client = new RazorpayClient(key, secret);

            Dictionary<string, string> options =
                new Dictionary<string, string>();

            options.Add("razorpay_order_id", orderId);
            options.Add("razorpay_payment_id", paymentId);
            options.Add("razorpay_signature", signature);

            Utils.verifyPaymentSignature(options);

            return true;
        }
    }
}