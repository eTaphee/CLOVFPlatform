using MediatR;
using PaginatedListDTO = CLOVFPlatform.Server.Services.DTO.PaginatedList<CLOVFPlatform.Server.Services.DTO.Employee>;

namespace CLOVFPlatform.Server.CQRS.Queries
{
    public class GetEmployeesQuery : IRequest<PaginatedListDTO>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string? Name { get; set; }
	}
}

