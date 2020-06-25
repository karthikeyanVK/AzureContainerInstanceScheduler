using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace OperationApps.Util
{
    public interface ITableStorageUtility
    {
       Task<List<T>> GetContainerScheduleDetails<T>() where T : ITableEntity, new();
    }
}
