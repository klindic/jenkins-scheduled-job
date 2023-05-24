using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Threading.Tasks;

class Program
{
    private static IConfigurationRoot CONFIG;

    static async Task Main(string[] args)
    {
        var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
        CONFIG = builder.Build();

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
        string apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        string fromEmailAddress = Program.CONFIG["SendGrid:FromEmailAddress"];

        var client = new SendGridClient(apiKey);

        var from = new EmailAddress(fromEmailAddress, "Sender");
        var subject = "Cancellation Notice";
        var to = new EmailAddress("jenkins-scheduled-job-test@yopmail.com", "Reciever");
        var plainTextContent = "Hello, this is to notify you that your order has been cancelled.";
        var htmlContent = "<strong>Hi there, it was nice to see you.</strong>";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine("Email was sent successfully.");
        }
        else
        {
            Console.WriteLine($"Failed to send email. Status code: {response.StatusCode}. Response body: {await response.Body.ReadAsStringAsync()}");
        }

    }
}
