using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindowsService_ExtractFromDatabaseIntoCsv
{
    class ErrorLogger
    {
        static string errorLogFile= "\\Logs\\ErrorLog.txt";
        static string filepath = AppDomain.CurrentDomain.BaseDirectory + errorLogFile;
        EmailClient emailClient = new EmailClient();

        public void LogError(Exception ex) 
        {
            var message = "Error: " + DateTime.Now.ToString("MM\\/dd\\/yyyy h\\:mm tt") + " : " + ex.Message;
            var emailSubject = "Error - CSV Extract" + DateTime.Now.ToString("MM\\/dd\\/yyyy h\\:mm tt");
            var emailBody = ex.Message;

            Console.WriteLine(ex.ToString());

            if (!File.Exists(filepath))
            {
                // Create the log file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {

                    sw.WriteLine(message);

                }
            }   else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(message);
                } 
            }

            //send email with error
            this.emailClient.SendEmail(emailSubject, emailBody);
        }
    }
}
