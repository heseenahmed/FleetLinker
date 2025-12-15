using FleetLinker.Domain.Entity.Dto;
using FleetLinker.Domain.IRepository;
using FleetLinker.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace FleetLinker.Infra.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _appDbContext;
        public DbSet<TEntity> _dbSet => _appDbContext.Set<TEntity>();
        public BaseRepository(ApplicationDbContext context) => _appDbContext = context;

        #region GetByID
        public TEntity GetByGuid(Guid Id)
        {
            return _dbSet.Find(Id)
                ?? throw new InvalidOperationException("Entity not found."); ;
        }

        public async Task<TEntity> GetByGuidAsync(Guid Guid)
        {
            return await _dbSet.FindAsync(Guid)
                ?? throw new InvalidOperationException("Entity not found."); ;
        }
        public async Task<TEntity> GetByGuidAsync(int Guid)
        {
            return await _dbSet.FindAsync(Guid)
                ?? throw new InvalidOperationException("Entity not found."); ;
        }
        public bool Exists(Guid Guid)
        {
            return _dbSet.Find(Guid) == null;
        }

        #endregion

        #region Get
        public TEntity? Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        public async Task<IEnumerable<TEntity>> GetListAsync()
        {
            return await _dbSet.ToListAsync();
        }
        //public async Task<IEnumerable<TEntity>> GetListWithIncludeAsync(Expression<Func<TEntity, bool>> predicate,
        //    params Expression<Func<TEntity, object>>[] includeProperties)
        //{
        //    IQueryable<TEntity> query = _dbSet;

        //    foreach (var includeProp in includeProperties)
        //    {
        //        query = query.Include(includeProp);
        //    }

        //    return await query.Where(predicate).ToListAsync();
        //}

        //public async Task<IEnumerable<TEntity>> GetListWithIncludeAsync(
        //           Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>,
        //               IIncludableQueryable<TEntity, object>>? include = null)
        //{
        //    IQueryable<TEntity> query = _dbSet;

        //    if (include != null)
        //    {
        //        query = include(query); // Apply Include and ThenInclude
        //    }

        //    if (predicate != null)
        //    {
        //        query = query.Where(predicate); // Apply filtering
        //    }

        //    return await query.ToListAsync();
        //}

        #endregion

        #region Add

        public int Add(TEntity entity)
        {
            _dbSet.Add(entity);
            return _appDbContext.SaveChanges();
        }
        public int AddRange(List<TEntity> entity)
        {
            _dbSet.AddRange(entity);
            return _appDbContext.SaveChanges();

        }

        public void AddWithoutSave(TEntity entity) => _dbSet.Add(entity);

        public void AddRangeWithoutSave(List<TEntity> entity) => _dbSet.AddRange(entity);

        public Task<int> AddAsync(TEntity entity)
        {
            _dbSet.AddAsync(entity);
            return _appDbContext.SaveChangesAsync();
        }

        public Task<int> AddRangeAsync(List<TEntity> entity)
        {
            _dbSet.AddRangeAsync(entity);
            return _appDbContext.SaveChangesAsync();
        }

        #endregion

        #region Update

        public int Update(TEntity entity)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;
            //   _dbSet.Update(entity);
            return _appDbContext.SaveChanges();
        }
        public void UpdateWithoutSave(TEntity entity) => _dbSet.Entry(entity).State = EntityState.Modified;

        public async Task<int> UpdateAsync(TEntity entity)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;

            var result = await _appDbContext.SaveChangesAsync();
            Console.WriteLine($"Rows affected: {result}");
            return result;

        }
        #endregion

        #region Delete
        public int Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
            return _appDbContext.SaveChanges();
        }

        public void RemoveWithoutSave(TEntity entity) => _dbSet.Remove(entity);

        public Task<int> RemoveAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            return _appDbContext.SaveChangesAsync();
        }

        #endregion

        #region Save_Dis
        public int Save()
        {
            return _appDbContext.SaveChanges();
        }
        public void Dispose()
        {
            _appDbContext.Dispose();
        }




        #endregion


        #region new Get methods
   //public async Task<PaginatedResult<TEntity>> GetPaginatedListWithIncludeAsync(
   //  Expression<Func<TEntity, bool>> predicate,
   //  Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
   //  int pageNumber = 1,
   //  int pageSize = 10,
   //  Expression<Func<TEntity, bool>>? searchExpression = null) // Add search expression
   //     {
   //         IQueryable<TEntity> query = _dbSet;

   //         // Apply Include and ThenInclude if provided
   //         if (include != null)
   //         {
   //             query = include(query);
   //         }

   //         // Apply filtering if predicate is provided
   //         if (predicate != null)
   //         {
   //             query = query.Where(predicate);
   //         }

   //         // Apply search filter if search expression is provided
   //         if (searchExpression != null)
   //         {
   //             query = query.Where(searchExpression);
   //         }

   //         // Get the total count of records
   //         var totalCount = await query.CountAsync();

   //         // Apply pagination
   //         var items = await query
   //             .Skip((pageNumber - 1) * pageSize)
   //             .Take(pageSize)
   //             .ToListAsync();

   //         // Return the paginated result
   //         return new PaginatedResult<TEntity>(items, totalCount, pageNumber, pageSize);
   //     }
        #endregion
    }
}