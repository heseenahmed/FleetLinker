
using BenzenyMain.Domain.Entity.Dto.Ads;
using MediatR;

namespace BenzenyMain.Application.Queries.Ads
{
    public record GetAllAdsQuery() : IRequest<AllAdsDto>;
    public record GetAllActiveAdsQuery() : IRequest<AdsDto>;
    public record GetSystemAdsQuery() : IRequest<AdsDto>;
    public record GetMobileAdsQuery() : IRequest<AdsDto>;
    public record GetAdsByIdQuery(Guid Id) : IRequest<AdsForGetDto>;

}
