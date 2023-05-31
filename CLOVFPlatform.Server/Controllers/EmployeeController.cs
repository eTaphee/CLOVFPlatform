using System;
using Microsoft.AspNetCore.Mvc;
using CLOVFPlatform.Server.Models;

namespace CLOVFPlatform.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> logger;

        public EmployeeController(ILogger<EmployeeController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetEmployeeAsync([FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "pageSize")] int pageSize = 1)
        {
            return Ok();
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Employee>> GetEmployeeAsync(string name)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployeeAsync()
        {
            return Created("", null);
        }
    }
}

