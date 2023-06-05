using CLOVFPlatform.Server.CQRS.Commands;
using CLOVFPlatform.Server.CQRS.Queries;
using CLOVFPlatform.Server.Services;
using CLOVFPlatform.Server.Services.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EmployeeDTO = CLOVFPlatform.Server.Services.DTO.Employee;

namespace CLOVFPlatform.Server.Controllers
{
    /// <summary>
    /// employee controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IMediator mediator;

        public EmployeeController(ILogger<EmployeeController> logger, IServiceProvider serviceProvider, IMediator mediator)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.mediator = mediator;
        }

        /// <summary>
        /// 직원 목록 조회
        /// </summary>
        /// <param name="page">페이지 위치</param>
        /// <param name="pageSize">페이지 항목 개수</param>
        /// <returns>직원 목록</returns>
        /// <response code="200">페이징 된 직원 목록 반환</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedList<EmployeeDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<EmployeeDTO>>> GetEmployeeAsync([FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "pageSize")] int pageSize = 5)
        {
            try
            {
                var query = new GetEmployeesQuery { Page = page, PageSize = pageSize };
                return Ok(await mediator.Send(query));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 이름으로 직원 정보 조회
        /// </summary>
        /// <param name="name">직원 이름</param>
        /// <param name="page">페이지 위치</param>
        /// <param name="pageSize">페이지 항목 개수</param>
        /// <returns>이름으로 필터링된 직원 목록</returns>
        /// <response code="200">페이징 된 직원 목록 반환</response>
        [HttpGet("{name}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedList<EmployeeDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetEmployeeAsync(string name, [FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "pageSize")] int pageSize = 5)
        {
            try
            {
                var query = new GetEmployeesQuery { Page = page, PageSize = pageSize, Name = name };
                return Ok(await mediator.Send(query));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 직원 정보 입력
        /// </summary>
        /// <param name="file">직원 정보 파일(json, csv)</param>
        /// <returns>추가된 직원 정보</returns>
        /// <exception cref="CLOVFPlatformException"></exception>
        /// <response code="201">페이징 된 직원 목록 반환(여러명인 경우 배열)</response>
        /// <response code="400">입력 매개변수에 문제가 있을 때</response>
        [HttpPost]
        [Consumes("multipart/form-data", "application/json", "text/csv")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(EmployeeDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostEmployeeAsync(IFormFile? file)
        {
            try
            {
                string ext = string.Empty; // file extension
                string? value = null; // value to parse
                IEmployeeParser? parser = null;

                if (string.IsNullOrWhiteSpace(Request.ContentType))
                {
                    throw new CLOVFPlatformException(400, "Content-Type Required");
                }

                if (Request.ContentType!.Contains("multipart/form-data"))
                {
                    if (file == null)
                    {
                        throw new CLOVFPlatformException(400, "Parameter Required", "file parameter required");
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

                if (Request.ContentType.Contains("application/json") || ext == ".json")
                {
                    parser = serviceProvider.GetService<IEmployeeJsonParser>()!;
                }
                else if (Request.ContentType.Contains("text/csv") || ext == ".csv")
                {
                    parser = serviceProvider.GetService<IEmployeeCsvParser>()!;
                }
                else
                {
                    throw new CLOVFPlatformException(400, "Invalid Format", $"{Request.ContentType} is not supported format");
                }

                if (parser!.IsValid(value!) == false)
                {
                    throw new CLOVFPlatformException(400, "Invalid Format", "content-type format invalid");
                }

                var models = parser!.GetEmployee(value!);
                var command = new CreateEmployeesCommand { Employees = models };
                var added = await mediator.Send(command);
                var addedCount = added.Count();

                if (addedCount > 1)
                {
                    return Created(Request.Path, added);
                }
                else if (addedCount == 1)
                {
                    var employee = added.First();
                    return Created($"{Request.Path}/{employee.Id}", employee);
                }

                return NoContent();
            }
            catch (Exception) 
            {
                throw;
            }
        }
    }
}

