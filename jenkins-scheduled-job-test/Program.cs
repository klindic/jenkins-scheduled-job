using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Assume we're cancelling an order in this example
        bool isOrderCancelled = CancelOrder(123);

        if (isOrderCancelled)
        {
            await SendNotificationAsync();
        }
    }

    // An example method to simulate cancelling an order or any other action.
    private static bool CancelOrder(int orderId)
    {
        Console.WriteLine($"Order {orderId} has been cancelled.");
        return true;
    }

    // Method to send email using SendGrid
    private static async Task SendNotificationAsync()
    {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("test@example.com", "Example User");
        var subject = "Cancellation Notice";
        var to = new EmailAddress("kristijanklindic@gmail.com", "Example User");
        var plainTextContent = "Hello, this is to notify you that your order has been cancelled.";
        var htmlContent = "<strong>Hello, this is to notify you that your order has been cancelled.</strong>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);
    }
}
