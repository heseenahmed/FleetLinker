using Benzeny.Domain.Entity.Dto;
using Benzeny.Infra.Data;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Car;
using BenzenyMain.Domain.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BenzenyMain.Infra.Repository
{
    public class CarRepository : ICarRepository
    {
        private readonly ApplicationDbContext _context;

        public CarRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Car car)
        {
            if (car is null) throw new ArgumentNullException(nameof(car));

            // ensure branch exists and is active
            var branchExists = await _context.Branches
                .AnyAsync(x => x.Id == car.BranchId && x.IsActive);
            if (!branchExists)
                throw new KeyNotFoundException("The specified branch does not exist or is inactive.");

            car.CreatedDate = DateTime.UtcNow;

            await _context.Cars.AddAsync(car);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddCarsAsync(List<Car> cars)
        {
            if (cars == null || cars.Count == 0)
                throw new ArgumentException("Car list is empty or null.");

            foreach (var car in cars)
            {
                car.CreatedDate = DateTime.UtcNow;
            }

            await _context.Cars.AddRangeAsync(cars);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Car car)
        {
            if (car is null) throw new ArgumentNullException(nameof(car));

            var existingCar = await _context.Cars.FirstOrDefaultAsync(c => c.Id == car.Id);
            if (existingCar == null)
                throw new KeyNotFoundException("Car not found.");

            // verify target branch if provided
            if (car.BranchId != Guid.Empty)
            {
                var branchExists = await _context.Branches.AnyAsync(b => b.Id == car.BranchId);
                if (!branchExists)
                    throw new KeyNotFoundException("Branch not found.");
            }

            // update fields
            existingCar.CarNumber = car.CarNumber ?? existingCar.CarNumber;
            existingCar.Color = car.Color ?? existingCar.Color;
            existingCar.Petroltype = car.Petroltype ?? existingCar.Petroltype;
            existingCar.CardNum = car.CardNum ?? existingCar.CardNum;
            existingCar.LicenseDate = car.LicenseDate ?? existingCar.LicenseDate;
            existingCar.CarModelId = car.CarModelId ?? existingCar.CarModelId;
            existingCar.CarBrandId = car.CarBrandId ?? existingCar.CarBrandId;
            existingCar.PlateTypeId = car.PlateTypeId ?? existingCar.PlateTypeId;
            existingCar.CarTypeId = car.CarTypeId ?? existingCar.CarTypeId;
            existingCar.BranchId = car.BranchId == Guid.Empty ? existingCar.BranchId : car.BranchId;
            existingCar.IsActive = car.IsActive;

            _context.Cars.Update(existingCar);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty) throw new ArgumentException("Invalid car ID.", nameof(id));

            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id);
            if (car == null)
                throw new KeyNotFoundException("Car not found.");

            _context.Cars.Remove(car);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Car?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid car ID.", nameof(id));

            return await _context.Cars
                .AsNoTracking()
                .Include(c => c.CarModel)
                .Include(c => c.CarBrand)
                .Include(c => c.CarType)
                .Include(c => c.PlateType)
                .Include(x => x.CarDrivers)
                    .ThenInclude(x => x.Driver)
                        .ThenInclude(x => x.User)
                .Include(c => c.Branch)
                    .ThenInclude(c => c.Company)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<PaginatedResult<CarForGet>> GetAllAsync(int pageNumber, int pageSize, string? searchTerm)
        {
            var query = _context.Cars
                .AsNoTracking()
                .Include(x => x.CarBrand)
                .Include(x => x.CarModel)
                .Include(x => x.CarType)
                .Include(x => x.PlateType)
                .Include(x => x.CarDrivers)
                    .ThenInclude(x => x.Driver)
                        .ThenInclude(x => x.User)
                .OrderBy(x => x.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.CarNumber.ToLower().Contains(term) ||
                    (c.Color != null && c.Color.ToLower().Contains(term)) ||
                    (c.Petroltype != null && c.Petroltype.Equals(term))
                );
            }

            var totalCount = await query.CountAsync();
            var activeCount = await query.Where(x => x.IsActive).CountAsync();
            var inactiveCount = await query.Where(x => !x.IsActive).CountAsync();

            var result = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CarForGet
                {
                    Id = c.Id,
                    CarNumber = c.CarNumber,
                    Color = c.Color ?? "",
                    CarModel = c.CarModel != null ? c.CarModel.Title : "",
                    CarBrand = c.CarBrand != null ? c.CarBrand.Title : "",
                    PlateType = c.PlateType != null ? c.PlateType.Title : "",
                    CarType = c.CarType != null ? c.CarType.Title : "",
                    LicenseDate = c.LicenseDate,
                    PetrolType = c.Petroltype,
                    DriversName = c.CarDrivers.Select(x => x.Driver.User.FullName).ToList(),
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return new PaginatedResult<CarForGet>(result, totalCount, pageNumber, pageSize, activeCount, inactiveCount);
        }

        public async Task<PaginatedResult<CarForGet>> GetCarsInBranch(Guid branchId, int pageNumber, int pageSize, string? searchTerm)
        {
            if (branchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.", nameof(branchId));

            var branchExists = await _context.Branches.AnyAsync(x => x.Id == branchId && x.IsActive);
            if (!branchExists)
                throw new KeyNotFoundException("Branch not found.");

            var query = _context.Cars
                .AsNoTracking()
                .Where(c => c.BranchId == branchId)
                .Include(x => x.CarBrand)
                .Include(x => x.CarModel)
                .Include(x => x.CarType)
                .Include(x => x.PlateType)
                .Include(x => x.CarDrivers)
                    .ThenInclude(x => x.Driver)
                        .ThenInclude(x => x.User)
                .OrderBy(x => x.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.CarNumber.ToLower().Contains(term) ||
                    (c.Color != null && c.Color.ToLower().Contains(term)) ||
                    (c.CarBrand != null && c.CarBrand.Title.ToLower().Contains(term)) ||
                    (c.CarModel != null && c.CarModel.Title.ToLower().Contains(term)) ||
                    (c.CarType != null && c.CarType.Title.ToLower().Contains(term)) ||
                    (c.PlateType != null && c.PlateType.Title.ToLower().Contains(term)) ||
                    (c.Petroltype != null && c.Petroltype.Equals(term))
                );
            }

            var totalCount = await query.CountAsync();
            var activeCount = await query.Where(x => x.IsActive).CountAsync();
            var inactiveCount = await query.Where(x => !x.IsActive).CountAsync();

            var result = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CarForGet
                {
                    Id = c.Id,
                    CarNumber = c.CarNumber,
                    Color = c.Color ?? "",
                    CarModel = c.CarModel != null ? c.CarModel.Title : "",
                    CarBrand = c.CarBrand != null ? c.CarBrand.Title : "",
                    PlateType = c.PlateType != null ? c.PlateType.Title : "",
                    CarType = c.CarType != null ? c.CarType.Title : "",
                    LicenseDate = c.LicenseDate,
                    PetrolType = c.Petroltype,
                    DriversName = c.CarDrivers.Select(x => x.Driver.User.FullName).ToList(),
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return new PaginatedResult<CarForGet>(result, totalCount, pageNumber, pageSize, activeCount, inactiveCount);
        }

        public async Task<bool> ImportCarsFromExcelAsync(IFormFile excelFile, string userId, Guid branchId)
        {
            if (excelFile == null || excelFile.Length == 0)
                throw new ArgumentException("Excel file is empty or null.");

            if (branchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.", nameof(branchId));

            // Verify branch exists and is active
            var branchExists = await _context.Branches.AnyAsync(b => b.Id == branchId && b.IsActive);
            if (!branchExists)
                throw new KeyNotFoundException("Branch not found or inactive.");

            var carsToCreate = new List<CarForCreateDto>();

            using var stream = new MemoryStream();
            await excelFile.CopyToAsync(stream);
            stream.Position = 0;

            using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed()?.RowsUsed().Skip(1); // Skip header row

            if (rows == null)
                return false;

            foreach (var row in rows)
            {
                var carNumber = row.Cell(1).GetString().Trim();
                if (string.IsNullOrWhiteSpace(carNumber) || await CarNumberExist(carNumber))
                    continue;

                // Safe parsing for optional fields
                int? carModelId = TryParseInt(row.Cell(2).GetString());
                int? carBrandId = TryParseInt(row.Cell(3).GetString());
                int? plateTypeId = TryParseInt(row.Cell(4).GetString());
                int? carTypeId = TryParseInt(row.Cell(5).GetString());
                string? color = row.Cell(6).GetString().Trim();
                string? cardNum = row.Cell(7).GetString().Trim();
                DateTime? licenseDate = TryParseDate(row.Cell(8).GetString());
                int? petrolType = TryParseInt(row.Cell(9).GetString());

                var dto = new CarForCreateDto
                {
                    CarNumber = carNumber,
                    CarModelId = carModelId,
                    CarBrandId = carBrandId,
                    PlateTypeId = plateTypeId,
                    CarTypeId = carTypeId,
                    Color = color,
                    CardNum = cardNum,
                    LicenseDate = licenseDate,
                    PetrolType = petrolType,
                    BranchId = branchId,
                    CreatedBy = userId,
                    IsActive = true
                };

                carsToCreate.Add(dto);
            }

            if (!carsToCreate.Any())
                return false;

            // Map DTOs to Entities
            var cars = carsToCreate.Select(dto => new Car
            {
                CarNumber = dto.CarNumber,
                CarModelId = dto.CarModelId,
                CarBrandId = dto.CarBrandId,
                PlateTypeId = dto.PlateTypeId,
                CarTypeId = dto.CarTypeId,
                Color = dto.Color,
                CardNum = dto.CardNum,
                LicenseDate = dto.LicenseDate,
                Petroltype = dto.PetrolType,
                BranchId = dto.BranchId,
                CreatedBy = dto.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                IsActive = dto.IsActive
            }).ToList();

            await _context.Cars.AddRangeAsync(cars);
            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<int> GetCarsCountInBranch(Guid branchId)
        {
            if (branchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.", nameof(branchId));

            return await _context.Cars.CountAsync(c => c.BranchId == branchId);
        }

        public async Task<bool> CarNumberExist(string carNumber)
        {
            if (string.IsNullOrWhiteSpace(carNumber))
                throw new ArgumentException("Car number must not be empty.", nameof(carNumber));

            return await _context.Cars.AnyAsync(x => x.CarNumber == carNumber);
        }

        public async Task<CarCompanyDto> GetCompanyCarStatusCountsAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.", nameof(companyId));

            // Use Contains with projected IDs to keep the query server-side
            var branchIds = _context.Branches
                .Where(b => b.CompanyId == companyId && b.IsActive)
                .Select(b => b.Id);

            var cars = _context.Cars.Where(c => branchIds.Contains(c.BranchId));

            var activeCars = await cars.CountAsync(c => c.IsActive);
            var inactiveCars = await cars.CountAsync(c => !c.IsActive);
            var branchesCount = await branchIds.CountAsync();

            return new CarCompanyDto
            {
                ActiveCars = activeCars,
                InactiveCars = inactiveCars,
                BranchesCount = branchesCount
            };
        }

        public async Task<bool> CarSwitchActive(Guid carId)
        {
            if (carId == Guid.Empty) throw new ArgumentException("Invalid car ID.", nameof(carId));

            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == carId)
                      ?? throw new KeyNotFoundException("Car not found.");

            car.IsActive = !car.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }
        #region Helpers
        private static int? TryParseInt(string? input)
        {
            return int.TryParse(input?.Trim(), out var result) ? result : null;
        }

        private static DateTime? TryParseDate(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // Accept common formats like yyyy-MM-dd, dd/MM/yyyy, etc.
            string[] formats = { "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy" };
            if (DateTime.TryParseExact(input.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            return null;
        }

        #endregion
    }
}
