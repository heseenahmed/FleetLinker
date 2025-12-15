using AutoMapper;
using BenzenyMain.Domain.Entity.Dto.Ads;
using BenzenyMain.Domain.Enum;
using BenzenyMain.Domain.IRepository;
using MediatR;

namespace BenzenyMain.Application.Queries.Ads
{
    public class AdsQueryHandler :
        IRequestHandler<GetAllAdsQuery, AllAdsDto>,
        IRequestHandler<GetAllActiveAdsQuery, AdsDto>,
        IRequestHandler<GetSystemAdsQuery, AdsDto>,
        IRequestHandler<GetMobileAdsQuery, AdsDto>,
        IRequestHandler<GetAdsByIdQuery, AdsForGetDto>
    {
        private readonly IAdsRepository _adsRepository;
        private readonly IMapper _mapper;

        public AdsQueryHandler(IAdsRepository adsRepository, IMapper mapper)
        {
            _adsRepository = adsRepository;
            _mapper = mapper;
        }

        public async Task<AllAdsDto> Handle(GetAllAdsQuery request, CancellationToken cancellationToken)
        {
            var (items, total, active, inactive, system, mobile) = await _adsRepository.GetAllWithStatsAsync();
            return new AllAdsDto
            {
                Items = _mapper.Map<List<AdsForGetDto>>(items),
                TotalAds = total,
                TotalAdsActive = active,
                TotalAdsDeActive = inactive,
                TotalSystemAds = system,
                TotalMobileAds = mobile
            };
        }

        public async Task<AdsDto> Handle(GetAllActiveAdsQuery request, CancellationToken cancellationToken)
        {
            var (items, total, active, inactive) = await _adsRepository.GetAllActiveWithStatsAsync();
            return new AdsDto
            {
                Items = _mapper.Map<List<AdsForGetDto>>(items),
                TotalAds = total,
                TotalAdsActive = active,
                TotalAdsDeActive = inactive
            };
        }

        public async Task<AdsDto> Handle(GetSystemAdsQuery request, CancellationToken cancellationToken)
        {
            var (items, total, active, inactive) = await _adsRepository.GetByTypeWithStatsAsync(AdvertisementType.System);
            return new AdsDto
            {
                Items = _mapper.Map<List<AdsForGetDto>>(items),
                TotalAds = total,
                TotalAdsActive = active,
                TotalAdsDeActive = inactive
            };
        }

        public async Task<AdsDto> Handle(GetMobileAdsQuery request, CancellationToken cancellationToken)
        {
            var (items, total, active, inactive) = await _adsRepository.GetByTypeWithStatsAsync(AdvertisementType.Mobile);
            return new AdsDto
            {
                Items = _mapper.Map<List<AdsForGetDto>>(items),
                TotalAds = total,
                TotalAdsActive = active,
                TotalAdsDeActive = inactive
            };
        }

        public async Task<AdsForGetDto> Handle(GetAdsByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new ArgumentException("Invalid advertisement ID.");

            var ad = await _adsRepository.GetByIdAsync(request.Id)
                     ?? throw new KeyNotFoundException("Advertisement not found.");

            return _mapper.Map<AdsForGetDto>(ad);
        }
    }
}
