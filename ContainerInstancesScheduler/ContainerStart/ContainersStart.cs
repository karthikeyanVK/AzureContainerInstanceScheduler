using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NCrontab;
using Newtonsoft.Json;
using OperationApps.Util;

namespace OperationsApps
{
    public class ContainersStart
    {
        private ITableStorageUtility tableStorageUtility;
        private IAzureContainerUtility azureContainerUtility;
        public ContainersStart(ITableStorageUtility tableStorageUtility, IAzureContainerUtility azureContainerUtility)
        {
            this.azureContainerUtility = azureContainerUtility;
            this.tableStorageUtility = tableStorageUtility;
        }

        [FunctionName("ContainersStart")]
        public void Run([TimerTrigger("%ContainerStartSchedule%")]
            TimerInfo startTimer, ExecutionContext context,
             ILogger log)
        {
            try
            {

                log.LogInformation($"ContainersStart function executed at: {DateTime.Now}");

                var containerScheduleDetails =
                tableStorageUtility.GetContainerScheduleDetails<ContainerScheduleDetail>().Result.Where(c => !c.isDisabled);

                foreach (var containerScheduleDetail in containerScheduleDetails)
                {
                    log.LogInformation($"containerScheduleDetail { JsonConvert.SerializeObject(containerScheduleDetail)}");
                    var cronSchedule = CrontabSchedule.Parse(containerScheduleDetail.StartSchedule);
                    var occurence =
                        cronSchedule.GetNextOccurrences(DateTime.Now.AddMinutes(-15), DateTime.Now.AddMinutes(15));

                    if (occurence != null && occurence.Any())
                    {
                        log.LogInformation($"ContainersStart function Started {containerScheduleDetail.ContainerImageName} at: {DateTime.Now}");

                        var result = azureContainerUtility.StartContainer(context.FunctionAppDirectory,
                             containerScheduleDetail.ResourceGroupName, containerScheduleDetail.ContainerGroupName);

                        log.LogInformation($"ContainersStart function completed {containerScheduleDetail.ContainerImageName} at: {DateTime.Now}");
                    }
                    else
                    {
                        log.LogInformation($"{containerScheduleDetail.ContainerImageName} does not have a schedule at: {DateTime.Now}");
                    }
                }

                log.LogInformation($"ContainersStart function Completed at: {DateTime.Now}");
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                throw;
            }
        }
    }
}
