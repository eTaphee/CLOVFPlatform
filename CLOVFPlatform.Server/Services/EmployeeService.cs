using System;
using System.Collections.Generic;
using AutoMapper;
using CLOVFPlatform.Server.Models;
using Microsoft.EntityFrameworkCore;
using EmployeeDTO = CLOVFPlatform.Server.Services.DTO.Employee;
using PaginatedListDTO = CLOVFPlatform.Server.Services.DTO.PaginatedList<CLOVFPlatform.Server.Services.DTO.Employee>;
using PaginatedLinkDTO = CLOVFPlatform.Server.Services.DTO.PaginatedLink;
using CLOVFPlatform.Server.Extensions;

namespace CLOVFPlatform.Server.Services
{
	/// <summary>
	/// 직원 정보 관리 서비스
	/// </summary>
	public interface IEmployeeService
	{
		/// <summary>
		/// 직원 정보 생성
		/// </summary>
		/// <param name="models">생성 할 직원정보</param>
		/// <returns>생성 된 직원 정보 목록(Email 중복 제거)</returns>
		Task<IEnumerable<EmployeeDTO>> CreateEmployeeAsync(IEnumerable<EmployeeDTO> models);

		/// <summary>
		/// 직원 정보 페이징 목록 반환
		/// </summary>
		/// <param name="page">페이지 위치</param>
		/// <param name="pageSize">페이지 크기</param>
		/// <param name="name">직원 이름 필,</param>
		/// <returns></returns>
		Task<PaginatedListDTO> GetEmployeeListAsync(int page = 1, int pageSize = 5, string? name = null);
	}

	public class EmployeeService : IEmployeeService
	{
		private readonly CLOVFContext context;
		private readonly IMapper mapper;
		private readonly IHttpContextAccessor httpContextAccessor;

        public EmployeeService(CLOVFContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
		{
			this.context = context;
			this.mapper = mapper;
			this.httpContextAccessor = httpContextAccessor;
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

		public async Task<PaginatedListDTO> GetEmployeeListAsync(int page = 1, int pageSize = 5, string? name = null)
		{
			try
			{
                page = page <= 0 ? 1 : page;
                pageSize = pageSize <= 0 ? 1 : pageSize;

				var query = context.Employee.OrderBy(m => m.Joined).ThenBy(m => m.Name).AsQueryable();

				if (!string.IsNullOrWhiteSpace(name))
				{
					query = query.Where(m => m.Name == name);
				}

				var dtoSource = query.Select(m => mapper.Map<EmployeeDTO>(m));

				var list = await PaginatedList<EmployeeDTO>.CreateAsync(dtoSource, page, pageSize);
				var link = list.GetPaginatedLinks(httpContextAccessor.HttpContext!.Request.getRequestUrl());

                return new PaginatedListDTO
				{
                    Page = page,
                    PageSize = pageSize,
                    Count = list.Count,
                    PageCount = list.PageCount,
                    TotalCount = list.TotalCount,
                    Links = mapper.Map<IEnumerable<PaginatedLinkDTO>>(link),
                    Items = list
                };
            }
			catch (Exception)
			{
				throw;
			}
		}

		public Task<EmployeeDTO> GetEmplyeeAsync()
		{
			throw new NotImplementedException();
		}
	}
}

