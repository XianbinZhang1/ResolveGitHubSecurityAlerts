using Microsoft.Extensions.Configuration;

namespace ContosoOrderProcessor.Configuration
{
    /// <summary>
    /// Application configuration management for ContosoOrderProcessor.
    /// 
    /// CONFIGURATION SOURCES IN THIS APPLICATION:
    /// This application uses .NET's hierarchical configuration system to load secrets from multiple sources.
    /// Configuration is loaded in the following order (later sources override earlier ones):
    ///   1. appsettings.json (base configuration and non-sensitive settings)
    ///   2. Environment variables (using double underscore __ separator for hierarchy, e.g., Aws__AccessKeyId)
    ///   3. Fallback to specific environment variable names (e.g., AWS_ACCESS_KEY_ID, GITHUB_TOKEN)
    /// 
    /// PRODUCTION ENVIRONMENTS - AZURE KEY VAULT:
    /// For PRODUCTION deployments, DO NOT use environment variables or hard-coded values.
    /// Instead, use Azure Key Vault for centralized, secure secret management.
    /// 
    /// SECURITY NOTE:
    /// This training application contains some hard-coded secrets (marked with SECURITY ISSUE comments)
    /// to demonstrate what NOT to do. In real applications, ALL secrets must be externalized.
    /// </summary>
    public static class AppConfig
    {
        private static IConfiguration? _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Application configuration constants
        public const string ApplicationName = "Contoso Order Processor";
        public const string ApplicationVersion = "2.1.0";
        public const string Environment = "Production";

        // AWS credentials loaded from environment variables or user secrets
        public static string AwsAccessKeyId => 
            _configuration?["Aws:AccessKeyId"] ?? 
            System.Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID") ?? 
            string.Empty;
        
        public static string AwsSecretAccessKey => 
            _configuration?["Aws:SecretAccessKey"] ?? 
            System.Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY") ?? 
            string.Empty;
        
        public const string AwsRegion = "us-west-2";
        public const string AwsS3Bucket = "contoso-order-documents";

        // Azure Storage connection string loaded from environment variables or user secrets
        public static string AzureStorageConnectionString =>
            _configuration?["Azure:StorageConnectionString"] ??
            System.Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ??
            string.Empty;

        // SendGrid API key loaded from environment variables or user secrets
        public static string SendGridApiKey =>
            _configuration?["SendGrid:ApiKey"] ??
            System.Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ??
            string.Empty;

        // Stripe API key loaded from environment variables or user secrets
        public static string StripeApiKey =>
            _configuration?["Stripe:ApiKey"] ??
            System.Environment.GetEnvironmentVariable("STRIPE_API_KEY") ??
            string.Empty;

        // Square Access Token loaded from environment variables or user secrets
        public static string SquareAccessToken =>
            _configuration?["Square:AccessToken"] ??
            System.Environment.GetEnvironmentVariable("SQUARE_ACCESS_TOKEN") ??
            string.Empty;

        // Mailgun API key loaded from environment variables or user secrets
        public static string MailgunApiKey =>
            _configuration?["Mailgun:ApiKey"] ??
            System.Environment.GetEnvironmentVariable("MAILGUN_API_KEY") ??
            string.Empty;

        // SMTP credentials loaded from environment variables or user secrets
        public static string SmtpHost =>
            _configuration?["Smtp:Host"] ??
            System.Environment.GetEnvironmentVariable("SMTP_HOST") ??
            string.Empty;

        public static string SmtpUsername =>
            _configuration?["Smtp:Username"] ??
            System.Environment.GetEnvironmentVariable("SMTP_USERNAME") ??
            string.Empty;

        public static string SmtpPassword =>
            _configuration?["Smtp:Password"] ??
            System.Environment.GetEnvironmentVariable("SMTP_PASSWORD") ??
            string.Empty;

        // SECURITY ISSUE: Hard-coded API keys for third-party services
        public const string TwilioAccountSid = "AC1234567890abcdef1234567890abcdef";
        public const string TwilioAuthToken = "1234567890abcdef1234567890abcdef";
        public const string TwilioPhoneNumber = "+1-555-0199";
        
        // SECURITY ISSUE: Hard-coded Slack tokens
        public const string SlackBotToken = "xoxb-1234567890123-1234567890123-abcdefghijklmnopqrstuvwx";
        public const string SlackWebhookUrl = "https://hooks.slack.com/services/T00000000/B00000000/XXXXXXXXXXXXXXXXXXXXXXXX";
        
        // GitHub Personal Access Token loaded from environment variables or user secrets
        public static string GitHubToken =>
            _configuration?["GitHub:PersonalAccessToken"] ??
            System.Environment.GetEnvironmentVariable("GITHUB_TOKEN") ??
            string.Empty;
        
        // npm token loaded from environment variables or user secrets
        public static string NpmToken =>
            _configuration?["Npm:Token"] ??
            System.Environment.GetEnvironmentVariable("NPM_TOKEN") ??
            string.Empty;

        // SECURITY ISSUE: Hard-coded JWT secret key
        public const string JwtSecretKey = "ThisIsAVerySecretKeyForJwtTokenGeneration2024!";
        public const int JwtExpirationMinutes = 60;

        // SECURITY ISSUE: Hard-coded encryption key
        public const string EncryptionKey = "E4B7C9D2F6A8E3C1B5D7F9A2C4E6B8D0";
        public const string EncryptionIV = "A1B2C3D4E5F6G7H8";

        // Application settings
        public const int MaxOrderRetries = 3;
        public const int OrderTimeoutSeconds = 30;
        public const decimal MinimumOrderAmount = 5.00m;
        public const decimal MaximumOrderAmount = 10000.00m;

        // Feature flags
        public const bool EnableEmailNotifications = true;
        public const bool EnableSmsNotifications = true;
        public const bool EnableOrderTracking = true;
        public const bool EnableFraudDetection = true;

        public static void PrintConfiguration()
        {
            Console.WriteLine("=== Application Configuration ===");
            Console.WriteLine($"Application: {ApplicationName} v{ApplicationVersion}");
            Console.WriteLine($"Environment: {Environment}");
            Console.WriteLine($"AWS Region: {AwsRegion}");
            Console.WriteLine($"AWS S3 Bucket: {AwsS3Bucket}");
            Console.WriteLine($"Email Notifications: {EnableEmailNotifications}");
            Console.WriteLine($"SMS Notifications: {EnableSmsNotifications}");
            Console.WriteLine($"Fraud Detection: {EnableFraudDetection}");
            Console.WriteLine("==================================\n");
        }

        public static string GetAwsCredentials()
        {
            return $"Access Key: {AwsAccessKeyId}, Secret Key: {AwsSecretAccessKey}";
        }

        public static bool ValidateConfiguration()
        {
            // Basic configuration validation
            if (string.IsNullOrEmpty(AwsAccessKeyId) || string.IsNullOrEmpty(AwsSecretAccessKey))
            {
                Console.WriteLine("[AppConfig] Warning: AWS credentials are not configured");
                return false;
            }

            if (string.IsNullOrEmpty(AzureStorageConnectionString))
            {
                Console.WriteLine("[AppConfig] Warning: Azure Storage connection string is not configured");
                return false;
            }

            Console.WriteLine("[AppConfig] Configuration validation passed");
            return true;
        }
    }
}
