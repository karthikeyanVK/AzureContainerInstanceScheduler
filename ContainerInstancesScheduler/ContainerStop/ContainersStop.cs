using System;
using System.IO;
using System.Linq;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace OperationsApps
{
    public static class ContainersStop
    {
        [FunctionName("ContainersStop")]
        public static void Run([TimerTrigger("%ContainerStopSchedule%")] TimerInfo stopTimer, ExecutionContext context, ILogger log)
        {
            log.LogInformation($"ContainersStop Timer trigger function executed at: {DateTime.Now}");
            //var resourceGroupName = "prod-mvp-operations-rg";

            IAzure azure = GetAzureContext(context);

            var containers = azure
                .ContainerGroups
                .List()
                .ToList();

            azure
               .ContainerGroups
               .GetById(containers.FirstOrDefault(x => x.Name == "prod-mvp-onemindindia-aci")?.Id)
               .StopAsync();

            log.LogInformation($"ContainersStop Timer trigger function completed at: {DateTime.Now}");
        }

        private static IAzure GetAzureContext(ExecutionContext context)
        {
            var azureAuthFile = Path.Combine(context.FunctionAppDirectory, "credentials.json");
            var azure = Azure.Authenticate(azureAuthFile).WithDefaultSubscription();

            return azure;
        }

    }
}
