
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity.Dto.Car;
using MediatR;

namespace BenzenyMain.Application.Queries.Car
{
    public class GetAllCarsQuery : IRequest<PaginatedResult<CarForGet>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }

        public GetAllCarsQuery(int pageNumber, int pageSize, string? searchTerm)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchTerm = searchTerm;
        }
    }
    public class GetCarByIdQuery : IRequest<CarForListDto>
    {
        public Guid CarId { get; set; }

        public GetCarByIdQuery(Guid carId)
        {
            CarId = carId;
        }
    }
    public class GetCarsInBranchQuery : IRequest<PaginatedResult<CarForGet>>
    {
        public Guid BranchId { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public string? Search { get; set; }

        public GetCarsInBranchQuery(Guid branchId, int pageNumber, int pageSize, string? search)
        {
            BranchId = branchId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            Search = search;
        }
    }
    public class GetCarsInCompanyQuery : IRequest<CarCompanyDto>
    {
        public Guid CompanyId { get; }

        public GetCarsInCompanyQuery(Guid companyId)
        {
            CompanyId = companyId;
        }
    }
}
