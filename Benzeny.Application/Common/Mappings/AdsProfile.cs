

using AutoMapper;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Ads;

namespace BenzenyMain.Application.Common.Mappings
{
    public class AdsProfile : Profile
    {
        public AdsProfile()
        {
            CreateMap<Ads, AdsForGetDto>()
               .ForMember(d => d.TypeId, opt => opt.MapFrom(s => ((int)s.Type)));
            //CreateMap<Ads, AdsDto>();
            CreateMap<CreateAdsDto, Ads>();
            CreateMap<UpdateAdsDto, Ads>()
                .ForMember(dest => dest.Image, opt => opt.Ignore()) 
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.StartDate, opt => opt.Ignore())
                .ForMember(dest => dest.EndDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());


        }
    }
}
