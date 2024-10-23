using LabInventASP.Infrastructure;
using LabInventASP.Interfaces;
using LabInventASP.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LabInventASP.Controllers
{
    [Route("api")]
    [ApiController]
    public class ValuesController(IRepository<DeviceStatus> repository) : ControllerBase
    {
        [HttpGet]
        public List<DeviceStatus> Get()
        {
            return repository.LoadAll();
        }
        // GET api/5
        [HttpGet("{ModuleState}")]
        public DeviceStatus Get([FromRoute] string ModuleState)
        {
            return repository.Load(ModuleState);
        }

        // POST api/
        [HttpPost]
        public ActionResult Post([FromBody] Models.DeviceStatus device)
        {
            if (device is null) return BadRequest();

            return Ok(repository.Save(device));
        }
    }
}
