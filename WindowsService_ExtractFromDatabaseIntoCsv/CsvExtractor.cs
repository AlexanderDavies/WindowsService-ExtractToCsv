using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;

namespace WindowsService_ExtractFromDatabaseIntoCsv
{

    class CsvExtractor
    {
        ErrorLogger errorLogger;
        List<Database> databases;
        string csvFilePath;
        StreamWriter sw;
        StringBuilder sb;
        int databaseCounter = 0;
        string delimiter = ",";

        public CsvExtractor()
        {
            this.errorLogger = new ErrorLogger();
            this.databases = new List<Database>();
            this.csvFilePath = AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyyMMdd") + "_Output.csv";
            this.sw = new StreamWriter(this.csvFilePath);
            this.sb = new StringBuilder();
        }

        //extract the data from the database into a string builder in CSV format;
        private void GetCsvFormattedData(Database database)
        {
            //Create the connection
            database.CreateConnection();

            // Open the connection.
            database.OpenConnection();

            // Retrieve data from database.
            database.ExtractData();

            //append headers to the string builder for first connection only.
            if (this.databaseCounter == 0)
            {
                           
                List<String> columnNames = Enumerable.Range(0, database.Reader.FieldCount).Select(database.Reader.GetName).ToList();

                this.sb.Append(string.Join(",", columnNames));

                this.sb.AppendLine();
            }

            //extract data from database reader into string builder.
            while (database.Reader.Read())
            {
                for (int i = 0; i < database.Reader.FieldCount; i++)
                {
                    string value = database.Reader[i].ToString();
                    if (value.Contains(this.delimiter))
                        value = "\"" + value + "\"";

                    this.sb.Append(value.Replace(Environment.NewLine, " ") + this.delimiter);
                }
                this.sb.Length--; // Remove the last comma
                this.sb.AppendLine();
            }

            database.Reader.Close();
            database.Connection.Close();
        }
            
        
        public void ExtractDataToCsv()
        {
         
            //get database config data
            NameValueCollection sqlSection = (NameValueCollection)ConfigurationManager.GetSection("sql");
            ConnectionStringSettingsCollection connectionStrings = ConfigurationManager.ConnectionStrings;

            //Create database object and push into a list of databases.
            if (connectionStrings != null)
            {
                foreach(ConnectionStringSettings cs in connectionStrings)
                {

                    if (sqlSection[cs.Name] != null)
                    {
                        Database newDatabase = new Database(cs.Name, cs.ProviderName, cs.ConnectionString, sqlSection[cs.Name]);
                        this.databases.Add(newDatabase);
                    }
                }
            }

            //for each database, create a connection and extract the data into a string.
            try
            {
                foreach (Database db in databases)
                {
                    GetCsvFormattedData(db);
                    this.databaseCounter++;
                }

                //write the data to CSV
                Console.WriteLine(sb.ToString());
                if (sb.ToString() != "")
                {
                    this.sw.Write(sb.ToString());
                    this.sw.Close();
                }
            }

            catch (Exception ex)
            {
                // Log errors and send email
                this.errorLogger.LogError(ex);
                this.sw.Close();

                // Delete the file if errors in any of the source systems
                File.Delete(csvFilePath);
            }
        }   
    }
}
