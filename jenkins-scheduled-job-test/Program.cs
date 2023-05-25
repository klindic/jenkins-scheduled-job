using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using jenkins_scheduled_job_test.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
        EmailConfig emailConfig = await GetSecrets();

        var client = new SendGridClient(emailConfig.ApiKey);

        var from = new EmailAddress(emailConfig.FromEmailAddress, "Sender");
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

    private static async Task<EmailConfig> GetSecrets()
    {
        string secretName = "jenkins-sheduled-job-test";
        string region = "us-east-1";

        var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

        var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);

        IAmazonSecretsManager client = new AmazonSecretsManagerClient(credentials, RegionEndpoint.GetBySystemName(region));


        GetSecretValueRequest request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT", // VersionStage defaults to AWSCURRENT if unspecified.
        };

        GetSecretValueResponse response;

        try
        {
            response = await client.GetSecretValueAsync(request);
        }
        catch (Exception e)
        {
            // For a list of the exceptions thrown, see
            // https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
            throw e;
        }

        var emailConfig = JsonConvert.DeserializeObject<EmailConfig>(response.SecretString);

        return emailConfig;
    }
}
