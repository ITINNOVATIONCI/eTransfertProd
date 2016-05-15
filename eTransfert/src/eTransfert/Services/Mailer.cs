using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FacebookAutomation.Services
{
    public class Mailer
    {



        public static void SendMail(MailerParametre parametre)
        {
            try
            {
                /* CREDENTIALS
                 * ===================================================*/
               

                string sgUsername = "";
                string sgPassword = "";
                /* MAIL SETTINGS
                 * Fill in the relevant information below.
                 * ===================================================*/
                // From
                // string fromAddress = "yverson@gmail.com";

                // To
                //string recipients = "boamathieu@yahoo.fr";

                // Subject
                //string subject = "Test";

                // Text Body
                //string text = "Hello,\n\nThis is a test message from SendGrid.    We have sent this to you because you requested a test message be sent from your account.\n\nThis is a link to google.com: http://www.google.com\nThis is a link to apple.com: http://www.apple.com\nThis is a link to sendgrid.com: http://www.sendgrid.com\n\nThank you for reading this test message.\n\nLove,\nYour friends at SendGrid";

                // HTML Body
                //string html = "<table style=\"border: solid 1px #000; background-color: #666; font-family: verdana, tahoma, sans-serif; color: #fff;\"> <tr> <td> <h2>Hello,</h2> <p>This is a test message from SendGrid.    We have sent this to you because you requested a test message be sent from your account.</p> <a href=\"http://www.google.com\" target=\"_blank\">This is a link to google.com</a> <p> <a href=\"http://www.apple.com\" target=\"_blank\">This is a link to apple.com</a> <p> <a href=\"http://www.sendgrid.com\" target=\"_blank\">This is a link to sendgrid.com</a> </p> <p>Thank you for reading this test message.</p> Love,<br/> Your friends at SendGrid</p> <p> <img src=\"http://cdn1.sendgrid.com/images/sendgrid-logo.png\" alt=\"SendGrid!\" /> </td> </tr> </table>";


                /* CREATE THE MAIL MESSAGE
                 * ===================================================*/
                var myMessage = new SendGridMessage();
                myMessage.AddTo(parametre.recipients);
                myMessage.From = new MailAddress(parametre.fromAddress);
                myMessage.Subject = parametre.subject;
                myMessage.Text = parametre.text;
                myMessage.Html = parametre.html;

                if (string.IsNullOrEmpty(parametre.templateEngine))
                {
                    
                }
                else
                {
                    // Enable template engine, you must send the template id
                    myMessage.EnableTemplateEngine(parametre.templateEngine);
                    //myMessage.AddSubstitution("iti_login", new List<string>() { "eTransfert" });
                    //myMessage.AddSubstitution("iti_password", new List<string>() { "p" });


                    try
                    {
                        foreach (var item in parametre.Substitution)
                        {
                            myMessage.AddSubstitution(item.Key, new List<string>() { item.Value });

                        }
                    }
                    catch (Exception)
                    {


                    }

                }

                
                




                /* SEND THE MESSAGE
                 * ===================================================*/
                var credentials = new NetworkCredential(sgUsername, sgPassword);
                // Create a Web transport for sending email.
                var transportWeb = new Web(credentials);

                // Send the email.
                transportWeb.DeliverAsync(myMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }




    }
}
