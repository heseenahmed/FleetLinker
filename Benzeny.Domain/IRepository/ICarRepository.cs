
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Car;
using Microsoft.AspNetCore.Http;

namespace BenzenyMain.Domain.IRepository
{
    public interface ICarRepository 
    {
        Task<PaginatedResult<CarForGet>> GetAllAsync(int pageNumber, int pageSize, string? searchTerm);
        Task<Car?> GetByIdAsync(Guid id);

        // Add: Car entity instead of DTO (mapping in handler)
        Task<bool> AddAsync(Car car);

        // Bulk add
        Task<bool> AddCarsAsync(List<Car> cars);

        // Update: Car entity instead of DTO
        Task<bool> UpdateAsync(Car car);

        // Soft delete
        Task<bool> DeleteAsync(Guid id);

        // Get cars in specific branch
        Task<PaginatedResult<CarForGet>> GetCarsInBranch(Guid branchId, int pageNumber, int pageSize, string? searchTerm);

        // Excel import
        Task<bool> ImportCarsFromExcelAsync(IFormFile excelFile, string userId, Guid branchId);


        // Get car count in branch
        Task<int> GetCarsCountInBranch(Guid branchId);

        // Check if car number exists
        Task<bool> CarNumberExist(string carNumber);

        // Get active/inactive car stats for a company
        Task<CarCompanyDto> GetCompanyCarStatusCountsAsync(Guid companyId);
        Task<bool> CarSwitchActive(Guid CarId);

    }
}
