using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


namespace OperationApps.Util
{
    public class TableStorageUtility: ITableStorageUtility
    {
        private static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;

            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine(
                    "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine(
                    "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;

            }

            return storageAccount;
        }
        public async Task<List<T>> GetContainerScheduleDetails<T>() where T : ITableEntity, new()
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                var cloudStorageAccount = CreateStorageAccountFromConnectionString(connectionString);
                var table = cloudStorageAccount.CreateCloudTableClient().GetTableReference("ContainerScheduleDetails");

                var containerScheduleDetails = new List<T>();

                var query = new TableQuery<T>();
                TableContinuationToken continuationToken = null;
                do
                {
                    var page = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                    continuationToken = page.ContinuationToken;
                    containerScheduleDetails.AddRange(page.Results);
                }
                while (continuationToken != null);

                return containerScheduleDetails;

            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

       
    }
}
