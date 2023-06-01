using System;
using Microsoft.AspNetCore.Mvc;
using CLOVFPlatform.Server.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using CLOVFPlatform.Server.Services;

namespace CLOVFPlatform.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> logger;
        private readonly CLOVFContext context;
        private readonly IServiceProvider serviceProvider;
        private readonly IEmployeeService employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, CLOVFContext context, IServiceProvider serviceProvider, IEmployeeService employeeService)
        {
            this.logger = logger;
            this.context = context;
            this.serviceProvider = serviceProvider;
            this.employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetEmployeeAsync([FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "pageSize")] int pageSize = 1)
        {
            return await context.Employee.ToListAsync();
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Employee>> GetEmployeeAsync(string name)
        {   
            return Ok();
        }

        void Test(string A)
        {

        }

        void Test(int A)
        {

        }

        [HttpPost]
        [Consumes("multipart/form-data", "application/json", "text/csv")]
        public async Task<ActionResult<Employee>> PostEmployeeAsync(IFormFile? file)
        {
            try
            {
                string ext = string.Empty; // file extension
                string? value = null; // value to parse
                IEmployeeParser? parser = null;

                if (Request.ContentType!.Contains("multipart/form-data"))
                {
                    if (file == null)
                    {
                        throw new Exception();
                    }

                    ext = System.IO.Path.GetExtension(file.FileName.ToLower());
                    using var sr = new StreamReader(file.OpenReadStream());
                    value = await sr.ReadToEndAsync();
                }
                else
                {
                    using var sr = new StreamReader(Request.Body);
                    value = await sr.ReadToEndAsync();
                }

                if (Request.ContentType == "application/json" || ext == ".json")
                {
                    parser = serviceProvider.GetService<IEmployeeJsonParser>()!;
                }
                else if (Request.ContentType == "text/csv" || ext == ".csv")
                {
                    parser = serviceProvider.GetService<IEmployeeCsvParser>()!;
                }

                if (parser!.IsValid(value!) == false)
                {
                    // throw
                }

                var models = parser!.GetEmployee(value!);
                var added = await employeeService.CreateEmployeeAsync(models);
                var addedCount = added.Count();

                if (addedCount > 1)
                {
                    return Created("/employee", null);
                }
                else if (addedCount == 1)
                {
                    var employee = added.First();
                    return Created($"/employee/{employee.Id}", null);
                }

                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //[HttpPost]
        //[Consumes("text/json")]
        //public async Task<ActionResult<Employee>> PostEmployeeAsync([FromBody] string body)
        //{
        //    return Ok();
        //}
    }
}

