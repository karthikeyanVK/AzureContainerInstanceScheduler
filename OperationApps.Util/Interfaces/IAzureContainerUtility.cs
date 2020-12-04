using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Azure.WebJobs;

namespace OperationApps.Util
{
    public interface IAzureContainerUtility
    {
        bool StartContainer(string credentialsPath, string resourceGroupName, string containerGroupName);
        bool StopContainer(string credentialsPath, string resourceGroupName, string containerGroupName);
    }
}
