using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GolfApplication.Data
{
    public class EmailSendGrid
    {
        public static string Apikey()  //Login userInfo
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            IConfigurationRoot configuration = builder.Build();
            var EmailKey = configuration.GetSection("EmailAPIKey").GetSection("Key").Value;

            return EmailKey;
        }


        public static async Task<string> Mail(string from, string to, string subject, string BodyContent)
        {
            try
            {
                var client = new SendGridClient(Apikey());
                SendGridMessage mail = new SendGridMessage();

               
                mail.From = new EmailAddress(from);
                
                if (to != null)
                {
                    mail.AddTo(to);                    
                }
                
                mail.Subject = subject;
                mail.PlainTextContent = BodyContent;

                var status = await client.SendEmailAsync(mail);
                return status.StatusCode.ToString();
            }

            catch (Exception e)
            {
                throw e;
            }

        }

        public static async Task<string> inviteMatchMail(string from, string to, string subject, string BodyContent)
        {
            try
            {
                var client = new SendGridClient(Apikey());
                SendGridMessage mail = new SendGridMessage();

                #region EmailTemplate for Content of the Mail
                string Body = string.Empty;

                mail.HtmlContent = BodyContent;
                #endregion

                //mail.From = new EmailAddress(from);
                //EmailAddress EmailAddress = new EmailAddress("Sunila@apptomate.co",null);

                //List<SendGrid.Helpers.Mail.EmailAddress> emails = new List<SendGrid.Helpers.Mail.EmailAddress>();
                //emails.Add(EmailAddress);
                string[] values = to.Split(',');
                var fromMail = new EmailAddress(from, "");

                //List<EmailAddress>toMail = new List<EmailAddress>();
                //toMail.Add("");
                //foreach (string i in values)
                //{
                //    dynamic tos = new List<EmailAddress>
                //   {
                //new EmailAddress(i, "Example User1"),
                //toMail.Add(tos)
                //     };
                //}
                var toMail = new List<EmailAddress>
            {
                new EmailAddress("sunila@apptmate.co", ""),
                new EmailAddress("murukeshs@apptomate.co", ""),
                new EmailAddress("kajas@apptomate.co", "")
            };
                //new EmailAddress("murukeshs@apptomate.co", "Example User2"),
                //new EmailAddress("kajas@apptomate.co", "Example User3")

                var showAllRecipients = false;

                //var tos = to.Select(x => MailHelper.StringToEmailAddress(x)).ToList();
                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(fromMail, toMail, subject, "", BodyContent, showAllRecipients);
                //if (tos != null)
                //{
                //    mail.AddTos(tos);
                //}

                mail.Subject = subject;
               // mail.PlainTextContent = BodyContent;

                var status = await client.SendEmailAsync(msg);
                return status.StatusCode.ToString();
            }

            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
