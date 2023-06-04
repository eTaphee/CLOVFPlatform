using AutoMapper;
using CLOVFPlatform.Server.CQRS.Queries;
using CLOVFPlatform.Server.Extensions;
using CLOVFPlatform.Server.Models;
using MediatR;
using EmployeeDTO = CLOVFPlatform.Server.Services.DTO.Employee;
using PaginatedLinkDTO = CLOVFPlatform.Server.Services.DTO.PaginatedLink;
using PaginatedListDTO = CLOVFPlatform.Server.Services.DTO.PaginatedList<CLOVFPlatform.Server.Services.DTO.Employee>;

namespace CLOVFPlatform.Server.CQRS.Handlers
{
    public class GetEmoloyeesQueryHandler : IRequestHandler<GetEmployeesQuery, PaginatedListDTO>
    {
        private readonly CLOVFContext context;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;

        public GetEmoloyeesQueryHandler(CLOVFContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
		{
            this.context = context;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
		}

        public async Task<PaginatedListDTO> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                int page = request.Page;
                int pageSize = request.PageSize;
                string? name = request.Name;

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
    }
}

