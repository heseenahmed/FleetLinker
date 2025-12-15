using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity.Dto.Driver;
using MediatR;

namespace BenzenyMain.Application.Queries.Driver
{
    public class GetAllDriversQuery : IRequest<PaginatedResult<DriverForListDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }

        public GetAllDriversQuery(int pageNumber, int pageSize, string? searchTerm)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchTerm = searchTerm;
        }
    }
    public class GetDriverByIdQuery : IRequest<DriverForGetIdDto>
    {
        public Guid DriverId { get; }

        public GetDriverByIdQuery(Guid driverId)
        {
            DriverId = driverId;
        }
    }
    public class GetDriversStatusInCompany : IRequest<DriverStatusDto>
    {
        public Guid CompanyId { get; }

        public GetDriversStatusInCompany(Guid companyId)
        {
            CompanyId = companyId;
        }
    }
    public class GetDriverInBranchQuery : IRequest<PaginatedResult<DriverForListDto>>
    {
        public Guid BranchId { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public string? SearchTerm { get; }

        public GetDriverInBranchQuery(Guid branchId, int pageNumber, int pageSize, string? searchTerm)
        {
            BranchId = branchId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchTerm = searchTerm;
        }
    }

}
