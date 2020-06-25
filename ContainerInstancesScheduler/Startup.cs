using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using OperationApps.Util;


[assembly: FunctionsStartup(typeof(ContainerInstancesScheduler.Startup))]
namespace ContainerInstancesScheduler
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            
            builder.Services.AddSingleton<ITableStorageUtility, TableStorageUtility>();
            builder.Services.AddSingleton<IAzureContainerUtility, AzureContainerUtility>();

        }
    }
}
