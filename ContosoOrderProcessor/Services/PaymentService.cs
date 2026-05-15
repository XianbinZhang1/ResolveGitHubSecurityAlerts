using ContosoOrderProcessor.Models;
using Microsoft.Extensions.Configuration;

namespace ContosoOrderProcessor.Services
{
    public class PaymentService
    {
        private readonly string _payPalClientId;
        private readonly string _payPalSecret;
        private readonly string _stripeApiKey;
        private readonly string _squareAccessToken;

        public PaymentService(IConfiguration? configuration = null)
        {
            // Load PayPal credentials from configuration or environment variables
            _payPalClientId = configuration?["PayPal:ClientId"] ?? 
                             Environment.GetEnvironmentVariable("PAYPAL_CLIENT_ID") ?? 
                             string.Empty;
            
            _payPalSecret = configuration?["PayPal:ClientSecret"] ?? 
                           Environment.GetEnvironmentVariable("PAYPAL_SECRET") ?? 
                           string.Empty;

            // Load Stripe API key from configuration or environment variables
            _stripeApiKey = configuration?["Stripe:ApiKey"] ?? 
                           Environment.GetEnvironmentVariable("STRIPE_API_KEY") ?? 
                           string.Empty;

            // Load Square Access Token from configuration or environment variables
            _squareAccessToken = configuration?["Square:AccessToken"] ?? 
                                Environment.GetEnvironmentVariable("SQUARE_ACCESS_TOKEN") ?? 
                                string.Empty;
        }

        public bool ProcessPayment(Order order)
        {
            try
            {
                Console.WriteLine($"[PaymentService] Processing payment for order {order.OrderId}");
                Console.WriteLine($"[PaymentService] Payment method: {order.PaymentMethod}");
                Console.WriteLine($"[PaymentService] Amount: ${order.TotalAmount}");

                bool paymentSuccessful = false;

                switch (order.PaymentMethod.ToLower())
                {
                    case "stripe":
                    case "credit card":
                        paymentSuccessful = ProcessStripePayment(order);
                        break;
                    case "paypal":
                        paymentSuccessful = ProcessPayPalPayment(order);
                        break;
                    default:
                        Console.WriteLine($"[PaymentService] Unknown payment method: {order.PaymentMethod}");
                        return false;
                }

                if (paymentSuccessful)
                {
                    Console.WriteLine($"[PaymentService] Payment processed successfully for order {order.OrderId}");
                    order.UpdateStatus("Payment Confirmed");
                }
                else
                {
                    Console.WriteLine($"[PaymentService] Payment failed for order {order.OrderId}");
                    order.UpdateStatus("Payment Failed");
                }

                return paymentSuccessful;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PaymentService] Error processing payment: {ex.Message}");
                return false;
            }
        }

        private bool ProcessStripePayment(Order order)
        {
            Console.WriteLine($"[PaymentService] Using Stripe API key: {(_stripeApiKey.Length > 15 ? _stripeApiKey.Substring(0, 15) + \"...\" : \"[Not configured]\")}...");
            Console.WriteLine($"[PaymentService] Creating Stripe payment intent for ${order.TotalAmount}");
            
            // Simulated Stripe API call
            // In a real application, this would call Stripe's API
            // var stripe = new StripeClient(StripeApiKey);
            // var paymentIntent = stripe.PaymentIntents.Create(...);
            
            Console.WriteLine($"[PaymentService] Stripe payment completed successfully");
            return true;
        }

        private bool ProcessPayPalPayment(Order order)
        {
            Console.WriteLine($"[PaymentService] Using PayPal credentials");
            Console.WriteLine($"[PaymentService] Client ID: {(_payPalClientId.Length > 20 ? _payPalClientId.Substring(0, 20) + \"...\" : \"[Not configured]\")}");
            Console.WriteLine($"[PaymentService] Creating PayPal order for ${order.TotalAmount}");
            
            // Simulated PayPal API call
            // In a real application, this would authenticate and call PayPal's API
            // var auth = PayPalAuth.GetToken(_payPalClientId, _payPalSecret);
            // var payment = PayPal.CreateOrder(...);
            
            Console.WriteLine($"[PaymentService] PayPal payment completed successfully");
            return true;
        }

        public string GenerateTransactionId()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        public bool RefundPayment(string orderId, decimal amount)
        {
            Console.WriteLine($"[PaymentService] Processing refund for order {orderId}: ${amount}");
            
            // Simulated refund logic
            Console.WriteLine($"[PaymentService] Refund processed successfully");
            return true;
        }
    }
}
