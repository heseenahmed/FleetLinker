
using Benzeny.Domain.Entity.Dto;
using Benzeny.Domain.IRepository;
using BenzenyMain.Domain.Entity;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace BenzenyMain.Domain.IRepository
{
    public interface IRegionRepository : IBaseRepository<Region>
    {
        Task<PaginatedResult<Region>> GetPaginatedListWithIncludeAsync(
           Expression<Func<Region, bool>> predicate,
           Func<IQueryable<Region>, IIncludableQueryable<Region, object>> include,
           int pageNumber,
           int pageSize,
           Expression<Func<Region, bool>>? searchExpression = null
       );

        Task<List<Region>> GetListWithIncludeAsync(
            Expression<Func<Region, bool>> predicate,
            Func<IQueryable<Region>, IIncludableQueryable<Region, object>> include
        );
        Task DeleteHardAsync(Region region);
    }
}
