using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MimeKit;
using MailKit.Net.Smtp;

namespace University.Models.Helper
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация University", "universityBSUIR@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.yandex.ru", 25, false);
                await client.AuthenticateAsync("universityBSUIR@yandex.ru", "12!Qaz23");
                try
                {
                    await client.SendAsync(emailMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                await client.DisconnectAsync(true);
            }
        }
    }
}