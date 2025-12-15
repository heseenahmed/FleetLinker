
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.City;

namespace BenzenyMain.Domain.IRepository
{
    public interface ICityRepository
    {
        Task<List<City>> GetCitiesByRegionIdAsync(Guid regionId);
    }
}
