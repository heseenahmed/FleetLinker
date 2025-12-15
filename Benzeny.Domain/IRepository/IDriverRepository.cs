
using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Driver;
using Microsoft.AspNetCore.Http;

namespace BenzenyMain.Domain.IRepository
{
    public interface IDriverRepository
    {
        Task<bool> CreateDriverWithUserAsync(Driver driver, ApplicationUser user);
        Task<(List<Driver> Drivers, int TotalCount , int ActiveCount , int InactiveCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm);
        Task<DriverForGetIdDto> GetByIdAsync(Guid id);
        Task<PaginatedResult<Driver>> GetDriversInBranch(Guid branchId, int pageNumber, int pageSize, string? searchTerm);
        Task<bool> DeleteAsync(Guid driverId);
        Task<bool> AssignDriverToCarAsync(CarDriverDto carDriver);
        Task<bool> UnassignDriverFromCarAsync(CarDriverDto carDriver);
        Task<bool> ImportDriverFromExcelAsync(IFormFile file, string createdByUserId, Guid branchId);
        Task<bool> DriverSwitchActive(Guid driverId);
        Task<bool> AssignAmountAndTransactionTypeToDriversAsync(AssignDriverFundsRequestDto request);
        Task<DriverStatusDto> GetDriversStatusInCompany(Guid companyId);
    }
}
