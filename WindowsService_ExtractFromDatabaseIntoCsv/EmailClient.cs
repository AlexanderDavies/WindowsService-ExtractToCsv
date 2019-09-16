using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Collections.Specialized;
using System.Configuration;

namespace WindowsService_ExtractFromDatabaseIntoCsv
{
    class EmailClient
    {
        SmtpClient emailClient;

        public EmailClient() {
            this.emailClient = new SmtpClient();
            this.emailClient.Host = ((NameValueCollection)ConfigurationManager.GetSection("smtpserver"))["smtpServer"];
            this.emailClient.EnableSsl = true;
            this.emailClient.Timeout = 10000;
            this.emailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            this.emailClient.UseDefaultCredentials = true;
        }

        public void SendEmail(string subject, string body)
        {
            MailMessage mm = new MailMessage();

            var fromAddress = ((NameValueCollection)ConfigurationManager.GetSection("smtpserver"))["fromEmail"];
            var toAddress = ((NameValueCollection)ConfigurationManager.GetSection("smtpserver"))["toEmail"];
            mm.From = new MailAddress(fromAddress);
            mm.To.Add(new MailAddress(toAddress));
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.Subject = subject;
            mm.Body = body;          
        }
    }
}
