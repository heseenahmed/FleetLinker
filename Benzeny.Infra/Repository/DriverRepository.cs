using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto;
using Benzeny.Infra.Data;
using BenzenyMain.Application.Common;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Driver;
using BenzenyMain.Domain.Enum;
using BenzenyMain.Domain.IRepository;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BenzenyMain.Infra.Repository
{
    public class DriverRepository : IDriverRepository
    {
        private readonly ILogger<DriverRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public DriverRepository(
            ILogger<DriverRepository> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> CreateDriverWithUserAsync(Driver driver, ApplicationUser user)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var branchExists = await _context.Branches.AnyAsync(x => x.Id == driver.BranchId && x.IsActive);
                if (!branchExists)
                    throw new KeyNotFoundException("Branch not found.");

                var createResult = await _userManager.CreateAsync(user, user.PhoneNumber!);
                if (!createResult.Succeeded)
                    throw new ApplicationException($"User creation failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

                const string roleName = "Driver";
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var roleResult = await _roleManager.CreateAsync(new ApplicationRole(roleName) { Id = Guid.NewGuid().ToString() });
                    if (!roleResult.Succeeded)
                        throw new ApplicationException($"Role creation failed: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }

                var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);
                if (!addToRoleResult.Succeeded)
                    throw new ApplicationException($"Adding role failed: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");

                driver.UserId = user.Id;
                _context.Drivers.Add(driver);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<(List<Driver> Drivers, int TotalCount, int ActiveCount, int InactiveCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm)
        {
            _logger.LogInformation("Getting all drivers with pageNumber={pageNumber}, pageSize={pageSize}, search={searchTerm}", pageNumber, pageSize, searchTerm);

            var query = _context.Drivers
                .Include(x => x.User)
                .Include(x => x.Branch).ThenInclude(b => b.Company)
                .Include(x => x.Tag)
                .Include(x => x.CarDrivers).ThenInclude(x => x.Car).ThenInclude(x => x.PlateType)
                .Include(x => x.FundingAssignments.Where(a => a.IsActive))
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearch = searchTerm.Trim().ToLower();
                query = query.Where(d => d.User.FullName!.ToLower().Contains(lowerSearch));
            }

            query = query.OrderBy(x => x.Id);

            var totalCount = await query.CountAsync();
            var totalActiveCount = await query.CountAsync(x => x.IsActive);
            var totalInactiveCount = await query.CountAsync(x => !x.IsActive);

            var drivers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (drivers, totalCount, totalActiveCount, totalInactiveCount);
        }

        public async Task<DriverForGetIdDto> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting driver details by ID: {Id}", id);

            var driver = await _context.Drivers
                .Include(x => x.User)
                .Include(x => x.Tag)
                .Include(x => x.Branch).ThenInclude(x => x.Company)
                .Include(x => x.CarDrivers).ThenInclude(x => x.Car).ThenInclude(x => x.PlateType)
                .Include(x => x.FundingAssignments.Where(a => a.IsActive))
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (driver == null)
            {
                _logger.LogWarning("Driver not found with ID: {Id}", id);
                throw new KeyNotFoundException("Driver not found.");
            }

            var active = driver.FundingAssignments.FirstOrDefault();
            var assignmentDto = active == null ? null : new DriverFundingAssignmentDto
            {
                Amount = active.Amount,
                TransactionType = active.TransactionType,
                LimitType = active.LimitType,
                Days = WeekDaysHelper.FromFlags(active.WeeklyDays),
                MonthlyDay = active.MonthlyDay
            };

            var firstCarDriver = driver.CarDrivers.FirstOrDefault();
            var carId = firstCarDriver?.CarId;
            var carPlate = firstCarDriver?.Car?.PlateType?.Title;

            var dto = new DriverForGetIdDto
            {
                Id = driver.Id,
                UserId = Guid.TryParse(driver.UserId, out var uid) ? uid : (Guid?)null,
                BranchId = driver.BranchId,
                CarId = carId,
                FullName = driver.User?.FullName ?? driver.User?.UserName ?? string.Empty,
                TagName = driver.Tag?.Title,
                BranchName = driver.Branch?.Company?.Name ?? string.Empty,
                License = driver.License,
                LicenseDegree = driver.LicenseDegree,
                ConsumptionType = null,
                Balance = driver.Balance?.ToString("0.00"),
                CardStatus = driver.CardStatus,
                PhoneNumber = driver.User?.PhoneNumber ?? string.Empty,
                CarPlate = carPlate,
                IsActive = driver.IsActive,
                FundingAssignment = assignmentDto
            };

            _logger.LogInformation("Driver details returned: {DriverId}", driver.Id);
            return dto;
        }

        public async Task<PaginatedResult<Driver>> GetDriversInBranch(Guid branchId, int pageNumber, int pageSize, string? searchTerm)
        {
            _logger.LogInformation("Getting drivers in branch {BranchId} with search {SearchTerm}", branchId, searchTerm);

            var branchExists = await _context.Branches.AnyAsync(b => b.Id == branchId && b.IsActive);
            if (!branchExists)
                throw new KeyNotFoundException("Branch not found.");

            var query = _context.Drivers
                .Where(d => d.BranchId == branchId)
                .Include(d => d.User)
                .Include(d => d.Tag)
                .Include(d => d.Branch).ThenInclude(b => b.Company)
                .Include(d => d.CarDrivers).ThenInclude(x => x.Car).ThenInclude(c => c.PlateType)
                .Include(x => x.FundingAssignments.Where(a => a.IsActive))
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearch = searchTerm.Trim().ToLower();
                query = query.Where(d =>
                    (d.User.FullName != null && d.User.FullName.ToLower().Contains(lowerSearch)) ||
                    (d.User.PhoneNumber != null && d.User.PhoneNumber.ToLower().Contains(lowerSearch)));
            }

            var totalCount = await query.CountAsync();
            var totalActiveCount = await query.CountAsync(x => x.IsActive);
            var totalInactive = await query.CountAsync(x => !x.IsActive);

            var drivers = await query
                .OrderBy(d => d.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Driver>(drivers, totalCount, pageNumber, pageSize, totalActiveCount, totalInactive);
        }

        public async Task<bool> DeleteAsync(Guid driverId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Starting hard delete for driver ID: {DriverId}", driverId);

                var driver = await _context.Drivers
                    .Include(d => d.User)
                    .FirstOrDefaultAsync(d => d.Id == driverId);

                if (driver == null)
                    throw new KeyNotFoundException("Driver not found.");

                var userId = driver.UserId;

                var carDriver = await _context.CarDriver.FirstOrDefaultAsync(cd => cd.DriverId == driverId);
                if (carDriver != null)
                {
                    var otherDrivers = await _context.CarDriver
                        .Where(cd => cd.CarId == carDriver.CarId && cd.DriverId != driverId)
                        .ToListAsync();

                    if (!otherDrivers.Any())
                        throw new InvalidOperationException("Cannot delete driver. The assigned car has no other drivers.");

                    var result = await UnassignDriverFromCarAsync(new CarDriverDto
                    {
                        DriverId = driverId,
                        CarId = carDriver.CarId
                    });

                    if (!result)
                        throw new ApplicationException("Failed to unassign driver from car.");
                }

                var userRoles = _context.UserRoles.Where(ur => ur.UserId == userId);
                _context.UserRoles.RemoveRange(userRoles);

                if (driver.User != null)
                {
                    _context.Users.Remove(driver.User);
                    _logger.LogInformation("User removed: {UserId}", userId);
                }

                _context.Drivers.Remove(driver);
                _logger.LogInformation("Driver removed: {DriverId}", driverId);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UnassignDriverFromCarAsync(CarDriverDto carDriver)
        {
            var entity = await _context.CarDriver
                .FirstOrDefaultAsync(cd => cd.DriverId == carDriver.DriverId && cd.CarId == carDriver.CarId);

            if (entity == null)
                throw new KeyNotFoundException("Assignment not found.");

            _context.CarDriver.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AssignDriverToCarAsync(CarDriverDto carDriver)
        {
            _logger.LogInformation("Assigning driver to car. DriverId: {DriverId}, CarId: {CarId}", carDriver.DriverId, carDriver.CarId);

            var driverExists = await _context.Drivers.AnyAsync(x => x.Id == carDriver.DriverId && x.IsActive);
            if (!driverExists)
                throw new KeyNotFoundException("Driver not found or inactive.");

            var carExists = await _context.Cars.AnyAsync(x => x.Id == carDriver.CarId && x.IsActive);
            if (!carExists)
                throw new KeyNotFoundException("Car not found or inactive.");

            var isDriverAssignedElsewhere = await _context.CarDriver
                .AnyAsync(cd => cd.DriverId == carDriver.DriverId && cd.CarId != carDriver.CarId);
            if (isDriverAssignedElsewhere)
                throw new ApplicationException("Driver is already assigned to another car.");

            var entity = new CarDriver
            {
                DriverId = carDriver.DriverId,
                CarId = carDriver.CarId,
            };

            await _context.CarDriver.AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ImportDriverFromExcelAsync(IFormFile file, string userId, Guid branchId)
        {
            _logger.LogInformation("Starting import of drivers from Excel.");

            if (file == null || file.Length == 0)
                throw new ArgumentException("Excel file is missing or empty.");

            var batchPhones = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var createDtos = new List<DriverForCreateDto>();

            // --- (اختياري لكن مفيد) جهّز Lookup للـ Tags مرة واحدة ---
            // يفترض وجود DbSet<Tag> فيه Id(int), Name(string)
            var tagsFromDb = await _context.Tags
                                           .AsNoTracking()
                                           .Select(t => new { t.Id, t.Title })
                                           .ToListAsync();
            var tagIdByName = tagsFromDb
                .GroupBy(t => t.Title?.Trim() ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            var usedRange = worksheet.RangeUsed();
            if (usedRange == null)
                throw new ApplicationException("No data found in Excel file.");

            var rows = usedRange.RowsUsed().ToList();
            if (rows.Count <= 1)
                throw new ApplicationException("No valid data rows found in Excel file.");

            // -------- Build header map (by name) --------
            var headerRow = rows.First();
            var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var cell in headerRow.Cells())
            {
                var headerText = cell.GetString()?.Trim();
                if (!string.IsNullOrWhiteSpace(headerText))
                    headerMap[headerText] = cell.Address.ColumnNumber; // 1-based
            }

            int Require(string name)
            {
                if (!headerMap.TryGetValue(name, out var col))
                    throw new ApplicationException($"Required column '{name}' not found in Excel header.");
                return col;
            }

            // Required in both old/new templates
            var COL_LICENSE = Require("License");
            var COL_LICENSE_DEGREE = Require("LicenseDegree");
            var COL_FULLNAME = Require("FullName");
            var COL_PHONE = Require("Phone");

            // Optional (support both templates)
            headerMap.TryGetValue("TagId", out var COL_TAGID);     // may be 0 if missing
            headerMap.TryGetValue("TagName", out var COL_TAGNAME); // may be 0 if missing

            // -------- Parse data rows --------
            foreach (var row in rows.Skip(1))
            {
                try
                {
                    string ReadStr(int col)
                    {
                        if (col <= 0) return string.Empty;
                        var cell = row.Cell(col);

                        // ClosedXML sometimes stores numbers; try formatted string first
                        var s = cell.GetFormattedString();
                        if (!string.IsNullOrWhiteSpace(s)) return s.Trim();

                        return cell.GetString()?.Trim() ?? string.Empty;
                    }

                    var license = ReadStr(COL_LICENSE);
                    var degree = ReadStr(COL_LICENSE_DEGREE);
                    var fullName = ReadStr(COL_FULLNAME);
                    var phone = ReadStr(COL_PHONE);

                    // Normalize phone (remove spaces)
                    phone = (phone ?? string.Empty).Replace(" ", string.Empty);

                    // Basic validation
                    if (string.IsNullOrWhiteSpace(fullName) ||
                        string.IsNullOrWhiteSpace(phone) ||
                        string.IsNullOrWhiteSpace(license) ||
                        string.IsNullOrWhiteSpace(degree))
                    {
                        _logger.LogWarning("Row skipped due to missing required fields.");
                        continue;
                    }

                    // batch duplicate check
                    if (!batchPhones.Add(phone))
                    {
                        _logger.LogWarning("Skipping duplicate phone in the same batch: {Phone}", phone);
                        continue;
                    }

                    // DB duplicate check
                    if (await DriverPhoneNumberExist(phone))
                    {
                        _logger.LogWarning("Skipping existing phone number in DB: {Phone}", phone);
                        continue;
                    }

                    // TagId resolution:
                    // 1) try TagId column if exists
                    int tagId = 0;
                    if (COL_TAGID > 0)
                    {
                        var raw = ReadStr(COL_TAGID);
                        if (!string.IsNullOrWhiteSpace(raw))
                        {
                            // accept numeric or text
                            int.TryParse(raw, out tagId);
                        }
                    }

                    // 2) if TagId still 0 and TagName present -> resolve from DB lookup
                    if (tagId <= 0 && COL_TAGNAME > 0)
                    {
                        var tagName = ReadStr(COL_TAGNAME);
                        if (!string.IsNullOrWhiteSpace(tagName) && tagIdByName.TryGetValue(tagName.Trim(), out var resolved))
                            tagId = resolved;
                    }

                    // Build DTO
                    var dto = new DriverForCreateDto
                    {
                        FullName = fullName,
                        phoneNumber = phone,
                        branchId = branchId,
                        TagId = tagId,       // 0 if no selection; adjust if you want to force required
                        License = license,
                        LicenseDegree = degree,
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow
                    };

                    createDtos.Add(dto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing Excel row. Skipping to next.");
                }
            }

            if (createDtos.Count == 0)
                throw new ApplicationException("No valid drivers found for import.");

            // -------- Map DTOs → Entities --------
            var drivers = new List<Driver>(createDtos.Count);
            foreach (var dto in createDtos)
            {
                var driver = new Driver
                {
                    BranchId = dto.branchId,
                    TagId = dto.TagId,
                    License = dto.License,
                    LicenseDegree = dto.LicenseDegree,
                    CreatedBy = dto.CreatedBy,
                    CreatedDate = dto.CreatedDate,
                    User = new ApplicationUser
                    {
                        FullName = dto.FullName,
                        UserName = BuildUserName(dto.FullName),
                        PhoneNumber = dto.phoneNumber
                    }
                };
                drivers.Add(driver);
            }

            await _context.Drivers.AddRangeAsync(drivers);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Drivers imported successfully. Count: {Count}", drivers.Count);
            return true;
        }


        private async Task<bool> DriverPhoneNumberExist(string phoneNumber)
        {
            _logger.LogInformation("Checking if driver phone number exists: {PhoneNumber}", phoneNumber);

            var exists = await _context.Drivers
                .Include(x => x.User)
                .AnyAsync(x => x.User != null && !x.User.Deleted && x.User.PhoneNumber == phoneNumber);

            _logger.LogInformation("Phone number exists: {Exists}", exists);
            return exists;
        }

        public async Task<bool> DriverSwitchActive(Guid driverId)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                var driver = await _context.Drivers
                    .Include(d => d.User)
                    .FirstOrDefaultAsync(d => d.Id == driverId);

                if (driver == null)
                    throw new KeyNotFoundException("Driver not found.");

                var willBeActive = !driver.IsActive;
                driver.IsActive = willBeActive;

                if (driver.User != null)
                    driver.User.Active = willBeActive;

                var assignmentsQuery = _context.Set<DriverFundingAssignment>()
                    .Where(a => a.DriverId == driverId);

                await assignmentsQuery.ExecuteUpdateAsync(s => s.SetProperty(a => a.IsActive, false));

                if (willBeActive)
                {
                    var newest = await assignmentsQuery
                        .OrderByDescending(a => a.CreatedDate)
                        .FirstOrDefaultAsync();

                    if (newest != null)
                        newest.IsActive = true;
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> AssignAmountAndTransactionTypeToDriversAsync(AssignDriverFundsRequestDto request)
        {
            ValidateAssignRequest(request);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            _logger.LogInformation("Assigning funds to drivers: {DriverIds}", string.Join(", ", request.DriversIds));

            var drivers = await _context.Drivers
                .Where(d => request.DriversIds.Contains(d.Id) && d.IsActive)
                .ToListAsync();

            if (!drivers.Any())
                throw new KeyNotFoundException("No active drivers found for the provided IDs.");

            var activeAssignments = await _context.Set<DriverFundingAssignment>()
                .Where(a => request.DriversIds.Contains(a.DriverId) && a.IsActive)
                .ToListAsync();

            var nowUtc = DateTime.UtcNow;

            foreach (var driver in drivers)
            {
                var prev = activeAssignments.FirstOrDefault(a => a.DriverId == driver.Id);
                if (prev != null) prev.IsActive = false;

                var assignment = new DriverFundingAssignment
                {
                    DriverId = driver.Id,
                    Amount = request.Amount,
                    TransactionType = request.TransactionType,
                    LimitType = null,
                    WeeklyDays = null,
                    MonthlyDay = null,
                    IsActive = true,
                    LastRunAtUtc = null,
                    NextRunAtUtc = nowUtc
                };

                if (request.TransactionType == TransactionType.Limit)
                {
                    switch (request.LimitType)
                    {
                        case LimitType.Daily:
                            assignment.LimitType = LimitType.Daily;
                            break;
                        case LimitType.Weekly:
                            if (request.Days == null || request.Days.Count == 0)
                                throw new ArgumentException("For weekly limit, 'Days' must contain at least one value.");
                            assignment.WeeklyDays = ToFlags(request.Days);
                            assignment.LimitType = LimitType.Weekly;
                            break;
                        case LimitType.Monthly:
                            assignment.MonthlyDay = (byte)Math.Clamp(nowUtc.Day, 1, 31);
                            assignment.LimitType = LimitType.Monthly;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(request.LimitType), "Unsupported limit type.");
                    }
                }

                assignment.CreatedBy = ""; // audit if needed

                // Immediate credit now
                driver.Balance = (driver.Balance ?? 0m) + assignment.Amount;
                assignment.LastRunAtUtc = nowUtc;

                if (assignment.TransactionType == TransactionType.OneTime)
                {
                    assignment.IsActive = false;
                    assignment.NextRunAtUtc = nowUtc;
                }
                else
                {
                    assignment.NextRunAtUtc = FirstRunUtcKeepTime(nowUtc, assignment);
                }

                _context.Set<DriverFundingAssignment>().Add(assignment);

                _logger.LogInformation(
                    "Saved assignment for DriverId {DriverId}: {TransactionType} {LimitType} {WeeklyDays} {MonthlyDay} Amount={Amount}",
                    driver.Id,
                    assignment.TransactionType,
                    assignment.LimitType?.ToString() ?? "-",
                    assignment.WeeklyDays?.ToString() ?? "-",
                    assignment.MonthlyDay?.ToString() ?? "-",
                    assignment.Amount
                );
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Successfully assigned funds to {Count} drivers", drivers.Count);
            return true;
        }

        public async Task<DriverStatusDto> GetDriversStatusInCompany(Guid companyId)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var dto = await _context.Drivers
                .Where(d => d.Branch.CompanyId == companyId)
                .GroupBy(_ => 1)
                .Select(g => new DriverStatusDto
                {
                    ActiveDrivers = g.Count(d => d.IsActive),
                    InActiveDrivers = g.Count(d => !d.IsActive)
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (dto is null)
                throw new KeyNotFoundException("Company not found or no drivers.");

            return dto;
        }

        #region Helpers

        private static void ValidateAssignRequest(AssignDriverFundsRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.DriversIds == null || request.DriversIds.Count == 0)
                throw new ArgumentException("DriversIds cannot be empty.");
            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");
            if (request.TransactionType == TransactionType.Limit && request.LimitType == null)
                throw new ArgumentException("LimitType is required when TransactionType is Limit.");
        }

        private static WeekDays ToFlags(IEnumerable<BenzenyMain.Domain.Enum.Days> days)
        {
            WeekDays flags = WeekDays.None;
            foreach (var d in days.Distinct())
            {
                flags |= d switch
                {
                    BenzenyMain.Domain.Enum.Days.Saturday => WeekDays.Saturday,
                    BenzenyMain.Domain.Enum.Days.Sunday => WeekDays.Sunday,
                    BenzenyMain.Domain.Enum.Days.Monday => WeekDays.Monday,
                    BenzenyMain.Domain.Enum.Days.Tuesday => WeekDays.Tuesday,
                    BenzenyMain.Domain.Enum.Days.Wednesday => WeekDays.Wednesday,
                    BenzenyMain.Domain.Enum.Days.Thursday => WeekDays.Thursday,
                    BenzenyMain.Domain.Enum.Days.Friday => WeekDays.Friday,
                    _ => WeekDays.None
                };
            }
            return flags;
        }

        private static DateTime FirstRunUtcKeepTime(DateTime nowUtc, DriverFundingAssignment a)
        {
            var tod = nowUtc.TimeOfDay;

            return a.LimitType switch
            {
                LimitType.Daily => nowUtc.Date.AddDays(1).Add(tod),
                LimitType.Weekly => NextWeeklyUtc(nowUtc, a.WeeklyDays ?? WeekDays.None, tod),
                LimitType.Monthly => NextMonthlyUtc(nowUtc, a.MonthlyDay ?? (byte)Math.Clamp(nowUtc.Day, 1, 31), tod),
                _ => nowUtc.Date.AddDays(1).Add(tod)
            };
        }

        private static DateTime NextWeeklyUtc(DateTime fromUtc, WeekDays days, TimeSpan tod)
        {
            if (days == WeekDays.None) return fromUtc.Date.AddDays(7).Add(tod);

            for (int i = 1; i <= 7; i++)
            {
                var date = fromUtc.Date.AddDays(i);
                if (HasFlag(days, date.DayOfWeek))
                    return date.Add(tod);
            }
            return fromUtc.Date.AddDays(7).Add(tod);
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

        private static DateTime NextMonthlyUtc(DateTime fromUtc, byte dom, TimeSpan tod)
        {
            var y = fromUtc.Year;
            var m = fromUtc.Month;

            if (fromUtc.Day >= dom)
            {
                m++;
                if (m > 12) { m = 1; y++; }
            }

            var last = DateTime.DaysInMonth(y, m);
            var day = Math.Min(dom, (byte)last);
            return new DateTime(y, m, day, 0, 0, 0, DateTimeKind.Utc).Add(tod);
        }
        private static string BuildUserName(string fullName)
        {
            var baseName = (fullName ?? "user").Trim();
            // Very light slugging; keep it simple for now
            var safe = new string(baseName.Where(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '.').ToArray());
            if (string.IsNullOrWhiteSpace(safe)) safe = "user";
            return $"{safe}_{Guid.NewGuid().ToString("N")[..6]}";
        }
        #endregion
    }
}
