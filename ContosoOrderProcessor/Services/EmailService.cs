using ContosoOrderProcessor.Models;
using Microsoft.Extensions.Configuration;

namespace ContosoOrderProcessor.Services
{
    public class EmailService
    {
        private readonly string _sendGridApiKey;
        private readonly string _mailgunApiKey;
        private readonly string _smtpHost;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        public EmailService(IConfiguration? configuration = null)
        {
            // Load SendGrid API key from configuration or environment variables
            _sendGridApiKey = configuration?["SendGrid:ApiKey"] ?? 
                             Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ?? 
                             string.Empty;

            // Load Mailgun API key from configuration or environment variables
            _mailgunApiKey = configuration?["Mailgun:ApiKey"] ?? 
                            Environment.GetEnvironmentVariable("MAILGUN_API_KEY") ?? 
                            string.Empty;

            // Load SMTP credentials from configuration or environment variables
            _smtpHost = configuration?["Smtp:Host"] ?? 
                       Environment.GetEnvironmentVariable("SMTP_HOST") ?? 
                       string.Empty;

            _smtpUsername = configuration?["Smtp:Username"] ?? 
                          Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? 
                          string.Empty;

            _smtpPassword = configuration?["Smtp:Password"] ?? 
                           Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? 
                           string.Empty;
        }

        public bool SendOrderConfirmation(Order order, Customer customer)
        {
            try
            {
                Console.WriteLine($"[EmailService] Sending order confirmation email");
                Console.WriteLine($"[EmailService] Using SendGrid API key: {(_sendGridApiKey.Length > 20 ? _sendGridApiKey.Substring(0, 20) + "..." : "[Not configured]")}");
                Console.WriteLine($"[EmailService] To: {customer.Email}");
                Console.WriteLine($"[EmailService] Subject: Order Confirmation - {order.OrderId}");

                string emailBody = GenerateOrderConfirmationEmail(order, customer);
                
                // Simulated SendGrid API call
                // In a real application, this would use SendGrid's SDK
                // var client = new SendGridClient(_sendGridApiKey);
                // var msg = MailHelper.CreateSingleEmail(...);
                // var response = await client.SendEmailAsync(msg);

                Console.WriteLine($"[EmailService] Email sent successfully to {customer.Email}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] Error sending email: {ex.Message}");
                return false;
            }
        }

        public bool SendShippingNotification(Order order, Customer customer, string trackingNumber)
        {
            try
            {
                Console.WriteLine($"[EmailService] Sending shipping notification email");
                Console.WriteLine($"[EmailService] Using SMTP server: {_smtpHost}");
                Console.WriteLine($"[EmailService] SMTP username: {_smtpUsername}");
                Console.WriteLine($"[EmailService] To: {customer.Email}");
                Console.WriteLine($"[EmailService] Tracking number: {trackingNumber}");

                // Simulated SMTP email sending
                // In a real application, this would use SmtpClient
                // using (var smtpClient = new SmtpClient(_smtpHost))
                // {
                //     smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                //     smtpClient.Send(mailMessage);
                // }

                Console.WriteLine($"[EmailService] Shipping notification sent successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] Error sending shipping notification: {ex.Message}");
                return false;
            }
        }

        private string GenerateOrderConfirmationEmail(Order order, Customer customer)
        {
            var emailBody = $@"
Dear {customer.Name},

Thank you for your order!

Order Details:
- Order ID: {order.OrderId}
- Order Date: {order.OrderDate:yyyy-MM-dd HH:mm:ss}
- Total Amount: ${order.TotalAmount}
- Payment Method: {order.PaymentMethod}

Items Ordered:
{string.Join("\n", order.Items.Select(item => $"  - {item.ProductName} (Qty: {item.Quantity}) - ${item.Price * item.Quantity}"))}

Shipping Address:
{customer.ShippingAddress}

We will send you another email once your order ships.

Best regards,
Contoso Order Processor Team
";
            return emailBody;
        }

        public bool SendPaymentFailureNotification(Order order, Customer customer)
        {
            try
            {
                Console.WriteLine($"[EmailService] Sending payment failure notification");
                Console.WriteLine($"[EmailService] To: {customer.Email}");
                Console.WriteLine($"[EmailService] Order ID: {order.OrderId}");
                
                // Simulated email sending
                Console.WriteLine($"[EmailService] Payment failure notification sent");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] Error sending payment failure notification: {ex.Message}");
                return false;
            }
        }
    }
}
