using System.Text.Json;
using System.Text;
using DreamcoreHorrorGameEmailServer.Models;
using System.Net.Mail;
using System.Net;
using DreamcoreHorrorGameEmailServer.ConstantValues;

namespace DreamcoreHorrorGameEmailServer.Services;

public class EmailBackgroundService
(
    IConfiguration configuration,
    ILogger<EmailBackgroundService> logger,
    IRabbitMqConsumer rabbitMqConsumer,
    IJsonSerializerOptionsProvider jsonSerializerOptionsProvider
)
: IHostedService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<EmailBackgroundService> _logger = logger;

    private readonly IRabbitMqConsumer _rabbitMqConsumer = rabbitMqConsumer;
    private readonly IJsonSerializerOptionsProvider _jsonSerializerOptionsProvider = jsonSerializerOptionsProvider;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ConfigureConsumerAsync(cancellationToken);
        return Task.CompletedTask;
    }

    private async Task ConfigureConsumerAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Initiated RabbitMQ consumer configuration for {type}", GetType().Name);

        await _rabbitMqConsumer.StartConsumingFromAsync(exchange: "verification-emails", queue: "email-server");
        _rabbitMqConsumer.ReceivedMessage += SendVerificationEmailFromBytesAsync;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _rabbitMqConsumer.Dispose();
        return Task.CompletedTask;
    }

    private async void SendVerificationEmailFromBytesAsync(byte[] data)
    {
        _logger.LogInformation("SendEmail was called.");

        string dataString = Encoding.UTF8.GetString(data);

        try
        {
            VerificationEmailDTO? verificationEmailDTO = JsonSerializer.Deserialize<VerificationEmailDTO>(dataString, _jsonSerializerOptionsProvider.Default);

            if (verificationEmailDTO is null)
                return;

            if (string.IsNullOrEmpty(verificationEmailDTO.To) ||
                string.IsNullOrEmpty(verificationEmailDTO.VerificationToken))
                return;

            string subject = "Please verify your email for Dreamcore Horror Game";

            string body = "<h3>Welcome to Dreamcore Horror Game!</h3>" +
                    $"<p>To finish setting up your account, please verify your email by clicking <a href=\"https://dreamcorehorrorgame.com/user/verifyEmail?token={verificationEmailDTO.VerificationToken}\">this link</a>.</p>" +
                    "<p>This is an automatically generated message. Replies are not monitored or answered.<br>If you have any questions, you can contact our support team: support@dreamcorehorrorgame.com.</p>";

            bool success = await SendEmailAsync(verificationEmailDTO.To, subject, body);

            _logger.LogInformation("Completed sending verification email to {recipient}.{newline}Status: {status}.",
                verificationEmailDTO.To, Environment.NewLine, success ? "success" : "fail");
        }
        catch (JsonException ex)
        {
            _logger.LogError
            (
                eventId: new EventId("SendVerificationEmailFromBytesAsync".GetHashCode() + ex.GetType().GetHashCode()),
                message: "JSON deserialization error occured while sending verification email from bytes.{newline}{message}", Environment.NewLine, ex.Message
            );
        }
    }

    private async Task<bool> SendEmailAsync(string toAddress, string subject, string body)
    {
        string? smtpHost = _configuration.GetSection(ConfigurationProperties.EmailService)[ConfigurationProperties.SmtpHost];
        string? smtpPortStr = _configuration.GetSection(ConfigurationProperties.EmailService)[ConfigurationProperties.SmtpPort];
        string? fromDisplayName = _configuration.GetSection(ConfigurationProperties.EmailService)[ConfigurationProperties.EmailDisplayName];
        string? fromAddress = _configuration.GetSection(ConfigurationProperties.EmailService)[ConfigurationProperties.EmailAddress];
        string? login = _configuration.GetSection(ConfigurationProperties.EmailService)[ConfigurationProperties.EmailLogin];
        string? password = _configuration.GetSection(ConfigurationProperties.EmailService)[ConfigurationProperties.EmailPassword];

        if (string.IsNullOrEmpty(smtpHost))
        {
            _logger.LogError
            (
                eventId: new EventId("SendEmail".GetHashCode() + ConfigurationProperties.SmtpHost.GetHashCode()),
                message: "Configuration property \"{property}\" was not found.", ConfigurationProperties.SmtpHost
            );

            return false;
        }

        if (string.IsNullOrEmpty(smtpPortStr))
        {
            _logger.LogError
            (
                eventId: new EventId("SendEmail".GetHashCode() + ConfigurationProperties.SmtpPort.GetHashCode()),
                message: "Configuration property \"{property}\" was not found.", ConfigurationProperties.SmtpPort
            );

            return false;
        }

        if (!int.TryParse(smtpPortStr, out int smtpPort))
        {
            _logger.LogError
            (
                eventId: new EventId("SendEmail".GetHashCode() + ConfigurationProperties.SmtpPort.GetHashCode() + 1),
                message: "Configuration property \"{property}\" has a value of incorrect type.", ConfigurationProperties.SmtpPort
            );

            return false;
        }

        if (string.IsNullOrEmpty(fromDisplayName))
        {
            _logger.LogError
            (
                eventId: new EventId("SendEmail".GetHashCode() + ConfigurationProperties.EmailDisplayName.GetHashCode()),
                message: "Configuration property \"{property}\" was not found.", ConfigurationProperties.EmailDisplayName
            );

            return false;
        }

        if (string.IsNullOrEmpty(fromAddress))
        {
            _logger.LogError
            (
                eventId: new EventId("SendEmail".GetHashCode() + ConfigurationProperties.EmailAddress.GetHashCode()),
                message: "Configuration property \"{property}\" was not found.", ConfigurationProperties.EmailAddress
            );

            return false;
        }

        if (string.IsNullOrEmpty(login))
        {
            _logger.LogError
            (
                eventId: new EventId("SendEmail".GetHashCode() + ConfigurationProperties.EmailLogin.GetHashCode()),
                message: "Configuration property \"{property}\" was not found.", ConfigurationProperties.EmailLogin
            );

            return false;
        }

        if (string.IsNullOrEmpty(password))
        {
            _logger.LogError
            (
                eventId: new EventId("SendEmail".GetHashCode() + ConfigurationProperties.EmailPassword.GetHashCode()),
                message: "Configuration property \"{property}\" was not found.", ConfigurationProperties.EmailPassword
            );

            return false;
        }

        MailAddress from = new(fromAddress, fromDisplayName);
        MailAddress to;

        try
        {
            to = new MailAddress(toAddress);
        }
        catch (FormatException ex)
        {
            _logger.LogError
            (
                eventId: new EventId("SendEmail".GetHashCode() + ex.GetType().GetHashCode()),
                message: "Failed to create a mail address from {toAddress}.{newline}{message}", toAddress, Environment.NewLine, ex.Message
            );

            return false;
        }

        MailMessage message = new(from, to)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        SmtpClient smtpClient = new()
        {
            Host = smtpHost,
            Port = smtpPort,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(login, password),
            Timeout = 10000
        };

        try
        {
            _logger.LogInformation("Sending an email message to {to} via SMTP client...", toAddress);

            await smtpClient.SendMailAsync(message);
        }
        catch (SmtpException ex)
        {
            _logger.LogError
            (
                eventId: new EventId("SendEmail".GetHashCode() + ex.GetType().GetHashCode()),
                message: "Failed to send an email via SMTP.{newline}{message}", Environment.NewLine, ex.Message
            );

            return false;
        }
        finally
        {
            message.Dispose();
            smtpClient.Dispose();
        }

        return true;
    }
}
