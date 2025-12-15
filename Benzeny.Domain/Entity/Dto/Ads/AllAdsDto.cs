
namespace BenzenyMain.Domain.Entity.Dto.Ads
{
    public class AllAdsDto
    {
        public List<AdsForGetDto>? Items { get; set; }
        public int TotalAds { get; set; } = 0;
        public int TotalAdsActive { get; set; } = 0;
        public int TotalAdsDeActive { get; set; } = 0;
        public int TotalSystemAds { get; set; } = 0;
        public int TotalMobileAds { get; set; } = 0;
    }
}
