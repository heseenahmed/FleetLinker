
using BenzenyMain.Domain.Enum;

namespace BenzenyMain.Domain.Entity.Dto.Ads
{
    public class AdsDto
    {
        public List<AdsForGetDto>? Items { get; set; }
        public int TotalAds { get; set; } = 0;
        public int TotalAdsActive { get; set; } = 0;
        public int TotalAdsDeActive { get; set; } = 0;
    }
}
