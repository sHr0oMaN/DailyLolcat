using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Common;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace WebMailer
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        private static CloudStorageAccount _storageAccount;

        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        public Program()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }

        static void Main()
        {
            var host = new JobHost();

            //TODO:
            // 1. Get email address (all) and today
            // 2. Get random img
            // 3. Send emails

            // Create the table client.
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            var allEmailAddresses = GetAllCoolPeople();

            CloudTable emailsSentTodayTable = tableClient.GetTableReference("emailsSent");
            var query = new TableQuery<EmailSent>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, DateTime.Today.ToString(CultureInfo.InvariantCulture)));
            var emailsAlreadySent = emailsSentTodayTable.ExecuteQuery(query).Select(e => e.EmailAddress);

            //IEnumerable<string> emailsYetToSend = allEmailAddresses.Except(emailsAlreadySent);

            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }

        private static IEnumerable<DynamicTableEntity> GetAllCoolPeople()
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("coolPeople");
            TableContinuationToken token = null;
            var entities = new List<DynamicTableEntity>();
            do
            {
                var queryResult = table.ExecuteQuerySegmented(new TableQuery(), token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);
            return table.ExecuteQuery(new TableQuery());
        }
    }
}
