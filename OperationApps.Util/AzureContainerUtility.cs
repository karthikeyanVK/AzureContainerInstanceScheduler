using System.IO;
using System.Linq;
using Microsoft.Azure.Management.Fluent;

namespace OperationApps.Util
{
    public class AzureContainerUtility : IAzureContainerUtility
    {
        public bool StartContainer(string credentialsPath, string resourceGroupName, string containerGroupName)
        {
            var azure = GetAzureContext(credentialsPath);
            azure.ContainerGroups.StartAsync(resourceGroupName,
                containerGroupName).GetAwaiter().GetResult();

            return true;
        }

        public bool StopContainer(string credentialsPath, string resourceGroupName, string containerGroupName)
        {
            IAzure azure = GetAzureContext(credentialsPath);

            var containers = azure
                .ContainerGroups
                .List().ToList();


            azure
               .ContainerGroups
               .GetById(containers.FirstOrDefault(x => x.Name == containerGroupName)?.Id)
               .StopAsync().GetAwaiter().GetResult();

            return true;
        }

        private static IAzure GetAzureContext(string credentialsPath)
        {
            var azureAuthFile = Path.Combine(credentialsPath, "credentials.json");
            var azure = Azure.Authenticate(azureAuthFile).WithDefaultSubscription();

            return azure;
        }
    }
    
}
