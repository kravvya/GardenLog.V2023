using GardenLog.SharedInfrastructure;
using System.Net.Mail;
using System.Text;

namespace UserManagement.Api.Data.ApiClients
{
    public interface IEmailClient
    {
        Task<bool> SendEmailAsync(SendEmailCommand request);
    }

    public class EmailClient : IEmailClient
    {
        private readonly ILogger<EmailClient> _logger;
        private readonly string _email_password;

        public EmailClient(IConfigurationService configurationService, ILogger<EmailClient> logger)
        {
            _logger = logger;
            _email_password = configurationService.GetEmailPassword();
        }

        public async Task<bool> SendEmailAsync(SendEmailCommand request)
        {
            try
            {
                MailMessage newMail = new MailMessage();
                // use the Gmail SMTP Host
                using (var client = new SmtpClient("smtp.mail.yahoo.com"))
                {

                    // Follow the RFS 5321 Email Standard
                    newMail.From = new MailAddress("gardenlog@yahoo.com","GardenLog Contact");

                    newMail.To.Add("stevchik@yahoo.com");

                    newMail.Subject = request.Subject;

                    newMail.IsBodyHtml = true;

                    StringBuilder sb = new();
                    sb.AppendLine($"From: {request.Name} <br/>");
                    sb.AppendLine($" at: {request.EmailAddress} <br/>");
                    sb.AppendLine($" sent message: {request.Message}");

                    newMail.Body = sb.ToString();

                    // enable SSL for encryption across channels
                    client.EnableSsl = true;
                    // Port 465 for SSL communication
                    client.Port = 587; // try port 587 if you experience issues)
                    // Provide authentication information with Gmail SMTP server to authenticate your sender account
                    client.Credentials = new System.Net.NetworkCredential("gardenlog@yahoo.com", _email_password);

                     await client.SendMailAsync(newMail);

                    return true;
                }
               
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception sending email", ex);
                return false;
            }
        }
    }
}
