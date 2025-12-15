
using Benzeny.Domain.Entity.Dto;
using Benzeny.Infra.Data;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Log;
using BenzenyMain.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using TimeZoneConverter;

namespace BenzenyMain.Infra.Repository
{
    public class LogRepository :ILogRepository
    {
        private readonly ApplicationDbContext _context;

        public LogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddLogAsync(Log log, CancellationToken ct = default)
        {
            await _context.Logs.AddAsync(log, ct);
            await _context.SaveChangesAsync(ct);
        }
        public async Task<PaginatedResult<LogForListDto>> GetLogsAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken ct = default)
            {
            var query = _context.Logs.AsNoTracking();

            // 🔍 Search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    l.PerformedBy.ToLower().Contains(searchTerm)
                );
            }

            // 🗓️ Date range filter
            if (fromDate.HasValue)
                query = query.Where(l => l.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(l => l.Timestamp <= toDate.Value);

            // 📊 Total count (after filters)
            var totalCount = await query.CountAsync(ct);

            // 🔁 Pagination
            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            // 🕐 Convert to Saudi Arabia time (12-hour format)
            var saudiZone = TZConvert.GetTimeZoneInfo("Asia/Riyadh");

            var items = logs.Select(l => new LogForListDto
            {
                Id = l.Id,
                Action = l.Action,
                PerformedBy = l.PerformedBy,
                Details = l.Details,
                Timestamp = l.Timestamp,
                TimestampFormatted = TimeZoneInfo
                    .ConvertTimeFromUtc(l.Timestamp, saudiZone)
                    .ToString("yyyy-MM-dd hh:mm:ss tt")
            }).ToList();

            // ✅ Return paginated result
            return new PaginatedResult<LogForListDto>(
                items,
                totalCount,
                pageNumber,
                pageSize,
                0, // ActiveCount (optional placeholder)
                0  // InActiveCount (optional placeholder)
            );
        }

    }
}
