using System.IO;
using Microsoft.Azure.Management.Fluent;

namespace OperationApps.Util
{
    public class AzureContainerUtility : IAzureContainerUtility
    {
        public bool StartContainer(string credentialsPath, string resourceGroupName, string containerGroupName)
        {
            var azure = GetAzureContext(credentialsPath);
            azure.ContainerGroups.StartAsync(resourceGroupName,
                containerGroupName);
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
