using Microsoft.WindowsAzure.Storage.Table;


namespace OperationsApps
{
    public class ContainerScheduleDetail : TableEntity
    {
        public string ContainerName { get; set; }
        public string ResourceGroupName { get; set; }
        public string ContainerImageName { get; set; }
        public string StartSchedule { get; set; }
        public string ContainerGroupName { get; set; }
        public bool isDisabled { get; set; }
        public string StopSchedule { get; set; }

    }
}
