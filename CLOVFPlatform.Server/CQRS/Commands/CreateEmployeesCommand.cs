using MediatR;
using EmployeeDTO = CLOVFPlatform.Server.Services.DTO.Employee;

namespace CLOVFPlatform.Server.CQRS.Commands
{
    public class CreateEmployeesCommand : IRequest<IEnumerable<EmployeeDTO>>
    {
        public IEnumerable<EmployeeDTO>? Employees { get; set; }
	}
}

