using Benzeny.Domain.Entity.Dto;
using Benzeny.Infra.Data;
using Benzeny.Infra.Repository;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace BenzenyMain.Infra.Repository
{
    public class RegionRepository : BaseRepository<Region>, IRegionRepository
    {
        private readonly ApplicationDbContext _context;

        public RegionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Region>> GetPaginatedListWithIncludeAsync(
            Expression<Func<Region, bool>> predicate,
            Func<IQueryable<Region>, IIncludableQueryable<Region, object>> include,
            int pageNumber,
            int pageSize,
            Expression<Func<Region, bool>>? searchExpression = null)
        {
            if (predicate == null) throw new ArgumentException("Predicate expression must not be null.");
            if (include == null) throw new ArgumentException("Include expression must not be null.");
            if (pageNumber < 1 || pageSize < 1) throw new ArgumentException("Invalid pagination parameters.");

            var query = _context.Regions.AsNoTracking().AsQueryable();
            query = include(query).Where(predicate);

            if (searchExpression != null)
                query = query.Where(searchExpression);

            var total = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Region>(items, total, pageNumber, pageSize);
        }

        public async Task<List<Region>> GetListWithIncludeAsync(
            Expression<Func<Region, bool>> predicate,
            Func<IQueryable<Region>, IIncludableQueryable<Region, object>> include)
        {
            if (predicate == null) throw new ArgumentException("Predicate expression must not be null.");
            if (include == null) throw new ArgumentException("Include expression must not be null.");

            return await include(_context.Regions.AsNoTracking())
                .Where(predicate)
                .ToListAsync();
        }

        public async Task DeleteHardAsync(Region region)
        {
            if (region == null) throw new ArgumentException("Region entity is required for deletion.");

            _context.Regions.Remove(region);
            await _context.SaveChangesAsync();
        }
    }
}
