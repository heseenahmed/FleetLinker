
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Enum;

namespace BenzenyMain.Domain.IRepository
{
    public interface IAdsRepository
    {
        Task<bool> CreateAsync(Ads ad);
        Task<bool> UpdateAsync(Ads ad);
        Task<bool> DeleteAsync(Ads ad);
        Task<Ads?> GetByIdAsync(Guid id);
        Task<(List<Ads> Items, int Total, int Active, int Inactive , int TotalSystemAds, int totalMobileAds)> GetAllWithStatsAsync();
        Task<(List<Ads> Items, int Total, int Active, int Inactive)> GetAllActiveWithStatsAsync();
        Task<(List<Ads> Items, int Total, int Active, int Inactive)> GetByTypeWithStatsAsync(AdvertisementType type);


    }
}
