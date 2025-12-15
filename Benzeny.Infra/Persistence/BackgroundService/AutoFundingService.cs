using Benzeny.Infra.Data;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

public class AutoFundingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AutoFundingService> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);

    public AutoFundingService(IServiceScopeFactory scopeFactory, ILogger<AutoFundingService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await ProcessDueAssignments(stoppingToken); }
            catch (Exception ex) { _logger.LogError(ex, "Auto-funding tick failed."); }
            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task ProcessDueAssignments(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var now = DateTime.UtcNow;

        // Only ACTIVE limit-assignments that are due
        var due = await db.Set<DriverFundingAssignment>()
            .Where(a => a.IsActive
                     && a.TransactionType == TransactionType.Limit
                     && a.NextRunAtUtc <= now)
            .OrderBy(a => a.NextRunAtUtc)
            .Take(500)
            .ToListAsync(ct);

        if (due.Count == 0) return;

        foreach (var a in due)
        {
            await using var tx = await db.Database.BeginTransactionAsync(ct);

            // Ensure driver is still active
            var driver = await db.Drivers.FirstOrDefaultAsync(d => d.Id == a.DriverId, ct);
            if (driver == null || !driver.IsActive)
            {
                // deactivate strategy if driver is inactive
                a.IsActive = false;
                await db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                continue;
            }

            // 1) Transfer
            driver.Balance = (driver.Balance ?? 0m) + a.Amount;

            // 2) Reschedule, keeping the SAME time-of-day as the current NextRunAtUtc
            var anchor = a.NextRunAtUtc;              // due time we just executed
            a.LastRunAtUtc = now;

            a.NextRunAtUtc = a.LimitType switch
            {
                LimitType.Daily => anchor.AddDays(1),                             // preserves TOD
                LimitType.Weekly => NextWeeklyFrom(anchor, a.WeeklyDays ?? WeekDays.None),
                LimitType.Monthly => NextMonthlyFrom(anchor, a.MonthlyDay ?? 1),
                _ => anchor.AddDays(1)
            };

            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            _logger.LogInformation("Funded Driver {DriverId} by {Amount}. Next run at {NextRunUtc}.",
                a.DriverId, a.Amount, a.NextRunAtUtc);
        }
    }

    // Same-day-of-time helpers anchored at previous NextRunAtUtc
    private static DateTime NextWeeklyFrom(DateTime prevRunUtc, WeekDays days)
    {
        var tod = prevRunUtc.TimeOfDay;
        var baseDate = prevRunUtc.Date;

        for (int i = 1; i <= 7; i++)
        {
            var date = baseDate.AddDays(i);
            if (HasFlag(days, date.DayOfWeek))
                return date.Add(tod);
        }
        return baseDate.AddDays(7).Add(tod);
    }

    private static DateTime NextMonthlyFrom(DateTime prevRunUtc, byte dom)
    {
        var tod = prevRunUtc.TimeOfDay;
        var y = prevRunUtc.Year;
        var m = prevRunUtc.Month;

        m++;
        if (m > 12) { m = 1; y++; }

        var last = DateTime.DaysInMonth(y, m);
        var day = Math.Min(dom, (byte)last);
        return new DateTime(y, m, day, 0, 0, 0, DateTimeKind.Utc).Add(tod);
    }

    private static bool HasFlag(WeekDays flags, DayOfWeek dow) => dow switch
    {
        DayOfWeek.Saturday => flags.HasFlag(WeekDays.Saturday),
        DayOfWeek.Sunday => flags.HasFlag(WeekDays.Sunday),
        DayOfWeek.Monday => flags.HasFlag(WeekDays.Monday),
        DayOfWeek.Tuesday => flags.HasFlag(WeekDays.Tuesday),
        DayOfWeek.Wednesday => flags.HasFlag(WeekDays.Wednesday),
        DayOfWeek.Thursday => flags.HasFlag(WeekDays.Thursday),
        DayOfWeek.Friday => flags.HasFlag(WeekDays.Friday),
        _ => false
    };
}
