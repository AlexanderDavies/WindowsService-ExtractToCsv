using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace WindowsService_ExtractFromDatabaseIntoCsv
{
    static class EncryptConfigurationFile
    {
       static ErrorLogger errorLogger = new ErrorLogger(); 
       static System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public static void EncryptConfigFile()
        {
            try
            {
                var connectionString = config.GetSection("connectionStrings");

                if (!connectionString.SectionInformation.IsProtected)
                {
                   // Encrypt config file.
                    connectionString.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");

                    // Save the encrypted section.
                    connectionString.SectionInformation.ForceSave = true;

                    config.Save(ConfigurationSaveMode.Full);
                }
                
            }
            catch (Exception ex)
            {
                errorLogger.LogError(ex);
            }
        }

        public static void UnencryptConfigFile()
        {
            try
            {
                var connectionString = config.GetSection("connectionStrings");

                if (connectionString.SectionInformation.IsProtected)
                {
                    // Remove encryption.
                    connectionString.SectionInformation.UnprotectSection();

                    // Save the unencrypted section.
                    connectionString.SectionInformation.ForceSave = true;

                    config.Save(ConfigurationSaveMode.Full);
                    
                }
            }
            catch (Exception ex)
            {
                errorLogger.LogError(ex);
            }
        }

    }
}
