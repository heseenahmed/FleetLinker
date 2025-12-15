using Benzeny.Infra.Data;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Ads;
using BenzenyMain.Domain.Enum;
using BenzenyMain.Domain.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BenzenyMain.Infra.Repository
{
    public class AdsRepository : IAdsRepository
    {
        private readonly ApplicationDbContext _context;

        public AdsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(Ads ad)
        {
            if (ad is null) throw new ArgumentNullException(nameof(ad));
            await _context.Ads.AddAsync(ad);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Ads ad)
        {
            if (ad is null) throw new ArgumentNullException(nameof(ad));
            _context.Ads.Update(ad);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Ads ad)
        {
            if (ad is null) throw new ArgumentNullException(nameof(ad));
            _context.Ads.Remove(ad);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Ads?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty) throw new ArgumentException("Invalid advertisement ID.", nameof(id));
            return await _context.Ads.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<(List<Ads> Items, int Total, int Active, int Inactive, int TotalSystemAds, int totalMobileAds)> GetAllWithStatsAsync()
        {
            var items = await _context.Ads
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            var total = items.Count;
            var active = items.Count(x => x.IsActive);
            var inactive = total - active;
            var totalSystemAds = items.Count(x => x.Type == AdvertisementType.System);
            var totalMobileAds = items.Count(x => x.Type == AdvertisementType.Mobile);

            return (items, total, active, inactive, totalSystemAds, totalMobileAds);
        }

        public async Task<(List<Ads> Items, int Total, int Active, int Inactive)> GetAllActiveWithStatsAsync()
        {
            var items = await _context.Ads
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            var total = items.Count;
            var active = items.Count(x => x.IsActive);
            var inactive = total - active;

            return (items, total, active, inactive);
        }

        public async Task<(List<Ads> Items, int Total, int Active, int Inactive)> GetByTypeWithStatsAsync(AdvertisementType type)
        {
            var items = await _context.Ads
                .AsNoTracking()
                .Where(x => x.Type == type)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            var total = items.Count;
            var active = items.Count(x => x.IsActive);
            var inactive = total - active;

            return (items, total, active, inactive);
        }
    }
}
