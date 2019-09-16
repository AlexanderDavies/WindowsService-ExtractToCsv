using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace WindowsService_ExtractFromDatabaseIntoCsv
{
    class Database
    {
        string name;
        string provider;
        DbConnection connection = null;
        string connectionString;
        DbCommand command = null;
        DbDataReader reader;
        string query;
        ErrorLogger errorLogger;

        public DbConnection Connection
        {

            get { return connection; }

            set { connection = value; }

        }

        public DbDataReader Reader
        {

            get { return reader; }

        }

        public Database(string name, string provider, string connectionString, string query)
        {
            this.name = name;
            this.provider = provider;
            this.connectionString = connectionString;
            this.query = query;
            this.errorLogger = new ErrorLogger();
        }


        public void CreateConnection()
        {

            // Create the DbProviderFactory and DbConnection.
            if (this.name != null)
            {
                try
                {
                    DbProviderFactory ProviderFactory = DbProviderFactories.GetFactory(this.provider);

                    this.connection = ProviderFactory.CreateConnection();
                    this.connection.ConnectionString = this.connectionString;
                    
                }
                catch (Exception ex)
                {
                    // Set the connection to null if it was created.
                    if (this.connection != null)
                    {
                        this.connection = null;
                    }

                    this.errorLogger.LogError(ex);
                }
            }
        }

        // Initiate the command inside the DB object
        private void createCommand()
        {
            try
            {
                if (this.connection != null)
                {
                    this.command = this.connection.CreateCommand();
                    this.command.CommandText = this.query;
                    this.command.CommandTimeout = 240;
                }
            }
            catch(Exception ex)
            {
                if (this.connection != null)
                {
                    this.connection = null;
                }
                this.errorLogger.LogError(ex);
            }
        }

        // Open the connection
        public void OpenConnection()
        {
            this.connection.Open();
            this.createCommand();
        }

        // Extract the data
        public void ExtractData() 
        {
            try
            {
                if (this.connection != null & this.command != null)
                {
                    this.reader = command.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                if (this.connection != null ||  this.command  != null)
                {
                    this.connection = null;
                    this.command = null;
                }

                this.errorLogger.LogError(ex);
            }
        }
    }
}
