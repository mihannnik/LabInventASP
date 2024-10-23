namespace FileParser.Models
{
    public class DeviceStatus
    {
        public required string ModuleCategoryID { get; set; }
        public required string ModuleState { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is DeviceStatus other)
            {
                return ModuleCategoryID == other.ModuleCategoryID && ModuleState == other.ModuleState;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ModuleCategoryID, ModuleState);
        }
    }
}
