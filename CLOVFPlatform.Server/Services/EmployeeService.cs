using System;
using AutoMapper;
using CLOVFPlatform.Server.Models;
using Microsoft.EntityFrameworkCore;
using EmployeeDTO = CLOVFPlatform.Server.Services.DTO.Employee;

namespace CLOVFPlatform.Server.Services
{
	public interface IEmployeeService
	{
		/// <summary>
		/// 직원 정보 생성
		/// </summary>
		/// <param name="models">생성 할 직원정보</param>
		/// <returns>생성 된 직원 정보 목록(Email 중복 제거)</returns>
		Task<IEnumerable<EmployeeDTO>> CreateEmployeeAsync(IEnumerable<EmployeeDTO> models);
		// Task<EmployeeDTO> GetEmplyeeAsync();
		// Task<List<EmployeeDTO>> GetEmployeeListAsync();
	}

	public class EmployeeService : IEmployeeService
	{
		private readonly CLOVFContext context;
		private readonly IMapper mapper;

		public EmployeeService(CLOVFContext context, IMapper mapper)
		{
			this.context = context;
			this.mapper = mapper;
		}

		public async Task<IEnumerable<EmployeeDTO>> CreateEmployeeAsync(IEnumerable<EmployeeDTO> models)
		{
			try
			{
				var added = new List<EmployeeDTO>();

				foreach (var model in models)
				{
                    var exists = await context.Employee.FirstOrDefaultAsync(m => m.Email.ToLower() == model.Email.ToLower());
					if (exists != null)
					{
						Console.WriteLine($"{model.Email} already exists.");
                    }
                    else
                    {
                        model.Id = model.Id ?? Guid.NewGuid().ToString();
                        var employee = mapper.Map<Employee>(model)!;
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

		public Task<List<EmployeeDTO>> GetEmployeeListAsync()
		{
			throw new NotImplementedException();
		}

		public Task<EmployeeDTO> GetEmplyeeAsync()
		{
			throw new NotImplementedException();
		}
	}
}

