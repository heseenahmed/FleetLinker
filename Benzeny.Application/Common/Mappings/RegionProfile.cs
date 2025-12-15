
using AutoMapper;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Region;

namespace BenzenyMain.Application.Common.Mappings
{
    public class RegionProfile : Profile
    {
        public RegionProfile()
        {
            CreateMap<Region, RegionForListDto>()
                .ForMember(dest => dest.Branches, opt => opt.MapFrom(src => src.Branches));

            CreateMap<RegionForCreateDto, Region>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Branches, opt => opt.Ignore());

            CreateMap<RegionForUpdateDto, Region>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Branches, opt => opt.Ignore());
        }
    }
}
