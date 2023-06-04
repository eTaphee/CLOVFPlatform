using AutoMapper;
using CLOVFPlatform.Server.CQRS.Commands;
using CLOVFPlatform.Server.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using EmployeeDTO = CLOVFPlatform.Server.Services.DTO.Employee;

namespace CLOVFPlatform.Server.CQRS.Handlers
{
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeesCommand, IEnumerable<EmployeeDTO>>
	{
        private readonly CLOVFContext context;
        private readonly IMapper mapper;

        public CreateEmployeeCommandHandler(CLOVFContext context, IMapper mapper)
		{
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDTO>> Handle(CreateEmployeesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var added = new List<EmployeeDTO>();

                foreach (var model in request.Employees!)
                {
                    var exists = await context.Employee.FirstOrDefaultAsync(m => m.Email!.ToLower() == model.Email!.ToLower());
                    if (exists != null)
                    {
                        Console.WriteLine($"{model.Email} already exists.");
                    }
                    else
                    {
                        model.Id = model.Id ?? Guid.NewGuid().ToString();
                        var employee = mapper.Map<Models.Employee>(model)!;
                        var entityEntry = await context.Employee.AddAsync(employee);
                        added.Add(mapper.Map<EmployeeDTO>(entityEntry.Entity));
                    }
                }

                Console.WriteLine($"{added.Count()} rows added.");
                await context.SaveChangesAsync();

                return added;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

