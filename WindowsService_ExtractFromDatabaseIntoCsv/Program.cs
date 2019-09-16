using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration.Install;
using System.Reflection;

namespace WindowsService_ExtractFromDatabaseIntoCsv
{
    public partial class Program : ServiceBase
    {

        //installer
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            if (System.Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                }
            }
            else
            {
                ServiceBase.Run(new Program());
            }
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
       
            File.AppendAllText(@"C:\Temp\error.txt", ((Exception)e.ExceptionObject).Message + ((Exception)e.ExceptionObject).InnerException.Message);
        }

       //declare timer
        Timer timer = new System.Timers.Timer();
        DateTime scheduleTime;

        public Program()
        {
            scheduleTime = DateTime.Today.AddDays(0).AddHours(15).AddMinutes(3); // Schedule to run once a day at 1.00 am in PROD

            //on deployment if the set time is earlier then the current time then add 24 hours from schedule time
            if (DateTime.Now > scheduleTime)
            {
                scheduleTime = scheduleTime.AddHours(24);
            }

        }

        protected override void OnStart(string[] args)
        {

            //encrypt the config file connection strings when the service starts

            EncryptConfigurationFile.EncryptConfigFile();

            WriteToFile("Service is started at " + DateTime.Now);
            timer.Enabled = true;
            timer.Interval = scheduleTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(onElapsedTimer);

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);

            //unencrypt the config file connection strings when the service stops
            EncryptConfigurationFile.UnencryptConfigFile();

            base.OnStop();
        }

        public void onElapsedTimer(object sender, System.Timers.ElapsedEventArgs e)
        {

            //Create a new CsvExtractor and execute the extract data method
            CsvExtractor CsvExtractor = new CsvExtractor();
            CsvExtractor.ExtractDataToCsv();

            //reset the timer to run every 24 hours  //24 * 60 * 60 * 1000
            if (timer.Interval != 5 * 60 * 1000)
            {
                timer.Interval = 5 * 60 * 1000;
            }  
        }
     
        //method to write to logs for on service start and on service stop
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create the log file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
