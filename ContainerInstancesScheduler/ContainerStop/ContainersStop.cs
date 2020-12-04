using System;
using System.IO;
using System.Linq;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NCrontab;
using Newtonsoft.Json;
using OperationApps.Util;

namespace OperationsApps
{
    public class ContainersStop
    {
        private ITableStorageUtility tableStorageUtility;
        private IAzureContainerUtility azureContainerUtility;
        public ContainersStop(ITableStorageUtility tableStorageUtility, IAzureContainerUtility azureContainerUtility)
        {
            this.azureContainerUtility = azureContainerUtility;
            this.tableStorageUtility = tableStorageUtility;
        }
        [FunctionName("ContainersStop")]
        public void Run([TimerTrigger("%ContainerStopSchedule%")] TimerInfo stopTimer, ExecutionContext context, ILogger log)
        {
            log.LogInformation($"ContainersStop Timer trigger function executed at: {DateTime.Now}");
            //var resourceGroupName = "prod-mvp-operations-rg";
            var containerScheduleDetails = tableStorageUtility.GetContainerScheduleDetails<ContainerScheduleDetail>().Result.ToList().Where(c => !string.IsNullOrEmpty(c.StopSchedule));

            foreach (var containerScheduleDetail in containerScheduleDetails)
            {
                log.LogInformation($"containerScheduleDetail { JsonConvert.SerializeObject(containerScheduleDetail)}");
                var cronSchedule = CrontabSchedule.Parse(containerScheduleDetail.StopSchedule);
                var occurence =
                    cronSchedule.GetNextOccurrences(DateTime.Now.AddMinutes(-15), DateTime.Now.AddMinutes(15));


                if (occurence != null && occurence.Any())
                {

                    log.LogInformation($"ContainersStop function Started {containerScheduleDetail.ContainerImageName} at: {DateTime.Now}");
                    var result = azureContainerUtility.StopContainer(context.FunctionAppDirectory,
                         containerScheduleDetail.ResourceGroupName, containerScheduleDetail.ContainerGroupName);

                    log.LogInformation($"ContainersStop function completed for {containerScheduleDetail.ContainerImageName} at: {DateTime.Now}");
                }
                else
                {
                    log.LogInformation($"{containerScheduleDetail.ContainerImageName} does not have a schedule at: {DateTime.Now}");
                }
            }

            log.LogInformation($"ContainersStop Timer trigger function completed at: {DateTime.Now}");

        }


    }
}
