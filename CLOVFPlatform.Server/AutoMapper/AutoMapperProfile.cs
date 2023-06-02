using AutoMapper;
using CLOVFPlatform.Server.Models;
using EmployeeDTO = CLOVFPlatform.Server.Services.DTO.Employee;
using PaginatedLinkDTO = CLOVFPlatform.Server.Services.DTO.PaginatedLink;

namespace CLOVFPlatform.Server.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
		public AutoMapperProfile()
		{
			CreateMap<EmployeeDTO, Employee>();
			CreateMap<Employee, EmployeeDTO>();

			CreateMap<PaginatedLink, PaginatedLinkDTO>();
        }
	}
}

