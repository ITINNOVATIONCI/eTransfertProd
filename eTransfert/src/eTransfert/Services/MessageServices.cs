using Mandrill;
using Mandrill.Models;
using Mandrill.Requests.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTransfert.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public static MandrillApi _mandrill = new MandrillApi("PqeG2o_2NPgYpX_PTLqAMg");
        public static string EmailFromAddress = "yverson@gmail.com";
        public static string EmailFromName = "eTransfert";

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var lst = new List<Mandrill.Models.EmailAddress>();
            lst.Add(new Mandrill.Models.EmailAddress(email));

            var task = _mandrill.SendMessage(new SendMessageRequest(new EmailMessage
            {
                FromEmail = EmailFromAddress,
                FromName = EmailFromName,
                Subject = subject,
                To = lst,
                Html = message
            }));

            return task;
        }

        public Task SendWelcomeEmail(string firstName, string email)
        {

            const string subject = "Bienvenue dans eTransfert";

            var emailMessage = new EmailMessage
            {
                FromEmail = EmailFromAddress,
                FromName = EmailFromName,
                Subject = subject,
                To = new List<Mandrill.Models.EmailAddress> { new EmailAddress(email) },
                Merge = true,
            };

            emailMessage.AddGlobalVariable("subject", subject);
            emailMessage.AddGlobalVariable("first_name", firstName);

            var task = _mandrill.SendMessageTemplate(new SendMessageTemplateRequest(emailMessage, "welcome_chimp", null));

            task.Wait();

            return task;
        }

        public Task SendSmsAsync(string number, string message)
        {
            HelperSMS.SendSMS(number, message);
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
