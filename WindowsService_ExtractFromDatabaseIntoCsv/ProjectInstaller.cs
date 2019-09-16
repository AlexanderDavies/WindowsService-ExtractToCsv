using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace WindowsService_ExtractFromDatabaseIntoCsv
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            //set the privileges
            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.Description = "Extract data from database/s into a CSV";
            serviceInstaller.DisplayName = "CsvDataExtract.Service";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "CsvDataExtract.Service";
            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }

    }
}
