# Azure Container Instance Scheduler

This repository is used for stopping and starting the container services. Since the container services does support any schedule by default, this repository was created. This repository is nothing but a azure function that starts and stops ACI to save cost. You can stop the ACI when it is not needed and start it when needed. 

This scheduler is based on the assumption that you have your docker images uploaded to ACR and docker image running in ACI. 

For Creating ACR Refer here - https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal

For uploading Docker Image in ACR - https://docs.microsoft.com/en-us/azure/container-registry/container-registry-quickstart-task-cli

For Creating ACI Refere here - https://docs.microsoft.com/en-us/azure/container-instances/container-instances-quickstart-portal

Create a new Azure storage, visit [here](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-create?toc=%2Fazure%2Fstorage%2Fblobs%2Ftoc.json&tabs=azure-portal) for more details

Navigate to Access keys and get the Connection string and save it. 
![storagekeys](/docs/storagekeys.png)

Create a new Table in the Azure portal of the Azure storage created. Navigate to Tables section, add new Table and enter table name as ContainerScheduleDetails and press OK.

![createtable](/docs/createtable.png)

You can deploy the ContainerInstancesScheduler Project into azure functions. On How to deploy azure functions, visit [here](https://tutorials.visualstudio.com/first-azure-function/publish)

Navigate to Configuration section and add application settings as below.

![azfuncconfiguration](/docs/azfuncconfiguration.png)

```
AzureWebJobsStorage - <Copied Connection String>

ContainerStartSchedule - 0 */10 * * * * - Runs Every 10 Minutes

ContainerStopSchedule -  0 */10 * * * * - Runs Every 10 Minutes
```

Connect the Table storage using Azure Storage Explorer. Connect to the Storage account using the connection string which is copied before. You can download the storage explorer [here](https://azure.microsoft.com/en-us/features/storage-explorer/)

Add the entry as below. 

![add-entity](/docs/add-entity.png)

```

ContainerGroupName - Enter any unique name, this is for your reference.

ContainerImageName - Image name available in ACR

ContainerName - Container name created in ACI

ResourceGroupName - Resource group name where your ACI Exists.

Schedule - 20 8 * * * (Runs Every day at 8:20 AM, see below for more details ) 
```

![scheduler](/docs/scheduler-config.png)
```

isDisabled - Available incase you want to stop the ACI to run.

```

Once you click on Insert a record will be created in the table and then your ACI will be automatically triggered and will run on schedule.

Local Development Testing

Rename the local.dev.settings.json to local.settings.json 
Build the application.
Add the details about the ACI in table storage
Run and test the application.











