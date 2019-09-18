# WindowsService-ExtractToCsv

On a recent project I was tasked with building a batch integration component to extract data from multiple databases into a single CSV for daily ingestion by a third party application. This seemingly simple task became the victim of common Enterprise practice - the perfect storm of changing requirements, restrictive costs and restrictive IT policies all came into play. 

I originally developed an SSIS package which was to be deployed onto an additional SQLaaS server on the company’s new Hybrid cloud Platform. Specs where drawn up, costing approved and development commenced. However, once I had finished developing the package the project team and business owner decided they no longer wanted to fund the additional server due to high operational expenses. Additionally, the SSIS package required the use of an enterprise scheduling tool (CTRL-M) which would take upwards of 2 months for another team to design and implement. In an Enterprise environment sometimes the most basic of tasks can take forever!

My second attempt came in the form of PowerShell script which was to be triggered daily via Task Scheduler on the existng app server hosting the third party application. The script took a couple of days to write and test locally, however when I went to deploy the script to the DEV environment I discovered that the infrastructure policies meant that the task scheduler was disabled in the VM pattern and that the PowerShell scripts had to be signed by a CA (not a deal breaker but more overhead!).

Finally I landed on a third and relatively simple workable solution: A windows service with an inbuilt scheduler.

## Features

This Windows service has the following features:

- Extract data from one or more databases into a single CSV.
- Trigger the data extract on a daily basis using a built in scheduler.
- Log error messages on the local server and send email notifications to a support inbox.
- Configurable database and SMTP values through app.config file
- Self Installer

### Prerequisites

.Net 4.0

### Installing

1. Change the relevant smtp and database connection elements in the app.config file
2. Open PowerShell (or command prompt) as administrator
3. Run the following command: ...(path)\WindowsService_ExtractFromDatabaseIntoCsv.exe --install
4. Run the following command: services.msc 
5. Find the service CsvDataExtract.service and open it
6. Optional: under 'Log On' tab select 'This account' and enter the relevant account credentials e.g. service account. 
7. Click 'Apply'
8. Under the general tab click 'Start' to start the service then click 'Ok'

## Built With

* [VisualStudio2012]

## Additional Comments

1. While technically the SQL code within app.config can contain the full query, It’s advisable to only use 'exec Stored Procedure' statements for security reasons. Additional security can be achieved by embedding a single query to extract the necessary connection strings and queries from a table in a control DB. In my opinion this alternate metadata driven approach is superior, however for simplicities sake this service does not include that pattern.

2. This service was deployed using a dev ops pipeline - BitBucket, Jenkins, Artifactory and Chef, but that is possibly a story for another repo.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
