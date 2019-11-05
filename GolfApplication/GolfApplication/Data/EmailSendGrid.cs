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

                #region EmailTemplate for Content of the Mail
                string Body = string.Empty;

                mail.HtmlContent = Body;
                #endregion

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


    }
}
