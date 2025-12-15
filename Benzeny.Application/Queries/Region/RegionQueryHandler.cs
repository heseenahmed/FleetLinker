using AutoMapper;
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity.Dto.Region;
using BenzenyMain.Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using static BenzenyMain.Application.Queries.Region.RegionQuery;

namespace BenzenyMain.Application.Queries.Region
{
    public class RegionQueryHandler :
        IRequestHandler<GetRegionList, PaginatedResult<RegionForListDto>>,
        IRequestHandler<GetRegionById, RegionForListDto>
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;

        public RegionQueryHandler(IRegionRepository regionRepository, IMapper mapper)
        {
            _regionRepository = regionRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<RegionForListDto>> Handle(GetRegionList request, CancellationToken cancellationToken)
        {
            if (request.PageNumber <= 0 || request.PageSize <= 0)
                throw new ArgumentException("Invalid pagination parameters.");

            Expression<Func<Domain.Entity.Region, bool>> predicate = r => true;

            Expression<Func<Domain.Entity.Region, bool>>? search = null;
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm;
                search = r => r.Title.Contains(term);
            }

            Func<IQueryable<Domain.Entity.Region>, IIncludableQueryable<Domain.Entity.Region, object>> include = query =>
                query.Include(r => r.Branches);

            var result = await _regionRepository.GetPaginatedListWithIncludeAsync(
                predicate,
                include,
                request.PageNumber,
                request.PageSize,
                search);

            if (result == null || result.Items == null)
                throw new InvalidOperationException("Failed to retrieve region list.");

            var data = _mapper.Map<List<RegionForListDto>>(result.Items);

            return new PaginatedResult<RegionForListDto>(data, result.TotalCount, result.PageNumber, result.PageSize);
        }

        public async Task<RegionForListDto> Handle(GetRegionById request, CancellationToken cancellationToken)
        {
            if (request.RegionId == Guid.Empty)
                throw new ArgumentException("Region ID is invalid.");

            var region = await _regionRepository.GetListWithIncludeAsync(
                r => r.Id == request.RegionId,
                query => query.Include(r => r.Branches));

            if (region == null || !region.Any())
                throw new KeyNotFoundException("Region not found with the given ID.");

            return _mapper.Map<RegionForListDto>(region.First());
        }
    }
}
