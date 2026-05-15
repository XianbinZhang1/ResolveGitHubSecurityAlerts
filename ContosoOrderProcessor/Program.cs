using ContosoOrderProcessor.Models;
using ContosoOrderProcessor.Services;
using ContosoOrderProcessor.Security;
using ContosoOrderProcessor.Configuration;
using Microsoft.Extensions.Configuration;

namespace ContosoOrderProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   Contoso Order Processor - E-Commerce Application     ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");

            try
            {
                // Build configuration from appsettings.json and environment variables
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                // Validate required configuration is present
                ValidateRequiredConfiguration(configuration);

                // Initialize AppConfig with configuration
                AppConfig.Initialize(configuration);

                // Initialize configuration
                AppConfig.PrintConfiguration();
                AppConfig.ValidateConfiguration();

                // Initialize services
                var databaseService = new DatabaseService(configuration);
                var paymentService = new PaymentService(configuration);
                var emailService = new EmailService(configuration);
                var securityValidator = new SecurityValidator();

                Console.WriteLine("=== Starting Order Processing Workflow ===\n");

                // Step 1: Retrieve customer information
                Console.WriteLine("--- Step 1: Customer Retrieval ---");
                int customerId = 1001;
                var customer = databaseService.GetCustomer(customerId);
                
                if (customer == null)
                {
                    Console.WriteLine("[ERROR] Failed to retrieve customer information");
                    return;
                }

                // Step 2: Validate customer
                Console.WriteLine("\n--- Step 2: Customer Validation ---");
                if (!securityValidator.ValidateCustomer(customer))
                {
                    Console.WriteLine("[ERROR] Customer validation failed");
                    return;
                }

                // Step 3: Create order
                Console.WriteLine("\n--- Step 3: Order Creation ---");
                var orderItems = new List<OrderItem>
                {
                    new OrderItem("Wireless Mouse", 29.99m, 2),
                    new OrderItem("USB-C Cable", 12.99m, 3),
                    new OrderItem("Laptop Stand", 49.99m, 1)
                };

                string orderId = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}";
                var order = new Order(orderId, customerId, orderItems, "Stripe");
                
                Console.WriteLine($"[Order Created] Order ID: {order.OrderId}");
                Console.WriteLine($"[Order Created] Total Amount: ${order.TotalAmount}");
                Console.WriteLine($"[Order Created] Items Count: {order.Items.Count}");

                // Step 4: Validate order
                Console.WriteLine("\n--- Step 4: Order Validation ---");
                if (!securityValidator.ValidateOrder(order))
                {
                    Console.WriteLine("[ERROR] Order validation failed");
                    return;
                }

                // Step 5: Fraud risk assessment
                Console.WriteLine("\n--- Step 5: Fraud Detection ---");
                if (!securityValidator.CheckFraudRisk(order, customer))
                {
                    Console.WriteLine("[WARNING] High fraud risk detected - manual review required");
                    order.UpdateStatus("Pending Review");
                }

                // Step 6: Process payment
                Console.WriteLine("\n--- Step 6: Payment Processing ---");
                bool paymentSuccess = paymentService.ProcessPayment(order);
                
                if (!paymentSuccess)
                {
                    Console.WriteLine("[ERROR] Payment processing failed");
                    emailService.SendPaymentFailureNotification(order, customer);
                    return;
                }

                // Step 7: Save order to database
                Console.WriteLine("\n--- Step 7: Database Persistence ---");
                bool orderSaved = databaseService.SaveOrder(order);
                
                if (!orderSaved)
                {
                    Console.WriteLine("[ERROR] Failed to save order to database");
                    // In a real application, we would initiate refund here
                    return;
                }

                // Step 8: Log transaction
                string transactionId = paymentService.GenerateTransactionId();
                databaseService.LogTransaction(order.OrderId, "PAYMENT", order.TotalAmount);
                Console.WriteLine($"[Transaction] Transaction ID: {transactionId}");

                // Step 9: Send confirmation email
                Console.WriteLine("\n--- Step 8: Email Notification ---");
                bool emailSent = emailService.SendOrderConfirmation(order, customer);
                
                if (emailSent)
                {
                    order.UpdateStatus("Confirmed");
                }

                // Step 10: Simulate shipping notification
                Console.WriteLine("\n--- Step 9: Shipping Notification ---");
                order.UpdateStatus("Shipped");
                string trackingNumber = $"TRK-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                emailService.SendShippingNotification(order, customer, trackingNumber);

                // Final summary
                Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
                Console.WriteLine("║              ORDER PROCESSING COMPLETED                ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════╝");
                Console.WriteLine($"\n✓ Order ID: {order.OrderId}");
                Console.WriteLine($"✓ Customer: {customer.Name} ({customer.Email})");
                Console.WriteLine($"✓ Total Amount: ${order.TotalAmount}");
                Console.WriteLine($"✓ Payment Method: {order.PaymentMethod}");
                Console.WriteLine($"✓ Status: {order.Status}");
                Console.WriteLine($"✓ Transaction ID: {transactionId}");
                Console.WriteLine($"✓ Tracking Number: {trackingNumber}");
                Console.WriteLine($"\nOrder processing completed successfully at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                Console.WriteLine("\n[INFO] This application contains intentional security vulnerabilities for training purposes.");
                Console.WriteLine("[INFO] Use GitHub Secret Scanning to identify exposed secrets in the codebase.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[FATAL ERROR] An unexpected error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        static void ValidateRequiredConfiguration(IConfiguration configuration)
        {
            var required = new Dictionary<string, string>
            {
                { "Aws:AccessKeyId", "Aws__AccessKeyId" },
                { "Aws:SecretAccessKey", "Aws__SecretAccessKey" },
                { "Azure:StorageConnectionString", "Azure__StorageConnectionString" },
                { "SendGrid:ApiKey", "SendGrid__ApiKey" },
                { "PayPal:ClientId", "PayPal__ClientId" },
                { "PayPal:ClientSecret", "PayPal__ClientSecret" },
                { "Stripe:ApiKey", "Stripe__ApiKey" },
                { "Square:AccessToken", "Square__AccessToken" },
                { "Mailgun:ApiKey", "Mailgun__ApiKey" },
                { "Smtp:Host", "Smtp__Host" },
                { "Smtp:Username", "Smtp__Username" },
                { "Smtp:Password", "Smtp__Password" },
                { "Database:ConnectionString", "Database__ConnectionString" },
                { "GitHub:PersonalAccessToken", "GitHub__PersonalAccessToken" },
                { "Npm:Token", "Npm__Token" }
            };

            var missing = required
                .Where(kvp => string.IsNullOrEmpty(configuration[kvp.Key]))
                .ToList();

            if (missing.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n❌ ERROR: Required configuration missing!\n");
                Console.ResetColor();
                
                Console.WriteLine("The following configuration values are required:");
                foreach (var kvp in missing)
                {
                    Console.WriteLine($"  • {kvp.Key} (environment variable: {kvp.Value})");
                }
                
                Console.WriteLine("\nTo configure these variables, run:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  . .\\setup-secrets.ps1");
                Console.ResetColor();
                Console.WriteLine("\nThen run this application again.");
                
                Environment.Exit(1);
            }
        }
    }
}
