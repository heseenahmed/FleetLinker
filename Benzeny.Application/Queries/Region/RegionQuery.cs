
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity.Dto.Region;
using MediatR;

namespace BenzenyMain.Application.Queries.Region
{
    public class RegionQuery
    {
        public record GetRegionList(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null) : IRequest<PaginatedResult<RegionForListDto>>;
        public record GetRegionById(Guid RegionId) : IRequest<RegionForListDto>;

    }
}
