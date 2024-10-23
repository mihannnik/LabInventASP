using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace LabInventASP.Models
{
    public class DeviceStatus
    {
        [NotNull]
        [Key]
        public required string ModuleCategoryID { get; set; }
        [NotNull] public required string ModuleState { get; set; }
    }
}
