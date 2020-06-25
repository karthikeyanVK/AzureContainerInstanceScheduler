using Microsoft.WindowsAzure.Storage.Table;


namespace OperationApps.Util
{
    public class ContainerScheduleDetail : TableEntity
    {
        public string ContainerName { get; set; }
        public string ResourceGroupName { get; set; }
        public string ContainerImageName { get; set; }
        public string Schedule { get; set; }
        public string ContainerGroupName { get; set; }
        public string isDisabled { get; set; }

    }
}
