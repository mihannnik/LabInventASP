using LabInventASP.Interfaces;
using LabInventASP.Models;
using Microsoft.EntityFrameworkCore;

namespace LabInventASP.Infrastructure
{
    public class DBRepository(SQLiteDBContext context) : IRepository<DeviceStatus>
    {
        public DeviceStatus Load(string id)
        {
            return context.DeviceStatuses.Find(id);
        }

        public List<DeviceStatus> LoadAll()
        {
            return context.DeviceStatuses.ToList();
        }

        public string Save(DeviceStatus device)
        {
            if (context.DeviceStatuses.Find(device.ModuleCategoryID) is DeviceStatus d)
            {
                d.ModuleState = device.ModuleState;
            }
            else
            {
                context.Add(device);
            }
            context.SaveChanges();
            return device.ModuleCategoryID;
        }
    }
}
