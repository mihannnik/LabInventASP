using LabInventASP.Interfaces;
using LabInventASP.Models;

namespace LabInventASP.Infrastructure
{
    public class TempRepository : IRepository<DeviceStatus>
    {
        private List<DeviceStatus> devices = new List<DeviceStatus>();
        public DeviceStatus Load(string id)
        {
            return devices.Find(x => x.ModuleCategoryID == id);
        }

        public List<DeviceStatus> LoadAll()
        {
            return devices;
        }

        public string Save(DeviceStatus device)
        {
            if (devices.Exists(x => x.ModuleCategoryID == device.ModuleCategoryID))
            {
                devices.Find(x => x.ModuleCategoryID == device.ModuleCategoryID).ModuleState = device.ModuleState;
            }
            else
            {
                devices.Add(device);
            }
            return device.ModuleCategoryID;
        }
    }
}
