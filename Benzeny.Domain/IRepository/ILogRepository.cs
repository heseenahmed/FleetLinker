
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Log;

namespace BenzenyMain.Domain.IRepository
{
    public interface ILogRepository
    {
        Task AddLogAsync(Log log, CancellationToken ct = default);
        Task<PaginatedResult<LogForListDto>> GetLogsAsync(
              int pageNumber,
              int pageSize,
              string? search,
              DateTime? fromDate,
              DateTime? toDate,
              CancellationToken ct = default);
    }
}
