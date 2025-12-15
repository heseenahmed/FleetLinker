using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Entity.Dto;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FleetLinker.Domain.IRepository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        #region Create_Update_Delet Async
        Task<int> AddAsync(TEntity entity);
        Task<int> AddRangeAsync(List<TEntity> entity);
        Task<int> UpdateAsync(TEntity entity);
        Task<int> RemoveAsync(TEntity entity);
        #endregion

        #region Create_Update_Delet
        int Add(TEntity entity);
        int AddRange(List<TEntity> entity);
        int Update(TEntity entity);
        int Remove(TEntity entity);

        void AddWithoutSave(TEntity entity);
        void AddRangeWithoutSave(List<TEntity> entity);
        void UpdateWithoutSave(TEntity entity);
        void RemoveWithoutSave(TEntity entity);
        #endregion

        #region Get 
        TEntity GetByGuid(Guid Guid);
        bool Exists(Guid Guid);
        Task<TEntity> GetByGuidAsync(Guid Guid);
        Task<TEntity> GetByGuidAsync(int Guid);

        TEntity? Get(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetListAsync();
        //Task<IEnumerable<TEntity>> GetListWithIncludeAsync(Expression<Func<TEntity, bool>> predicate,
        //     params Expression<Func<TEntity, object>>[] includeProperties);

        //Task<IEnumerable<TEntity>> GetListWithIncludeAsync(
        //               Expression<Func<TEntity, bool>> predicate,
        //               Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
        #endregion

        #region Save_Dis
        int Save();
        void Dispose();
        #endregion

        //#region new get methods
        //Task<PaginatedResult<TEntity>> GetPaginatedListWithIncludeAsync( Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        //                                                                 int pageNumber = 1,int pageSize = 10 , Expression<Func<TEntity, bool>>? searchExpression = null);
        //#endregion
    }
}