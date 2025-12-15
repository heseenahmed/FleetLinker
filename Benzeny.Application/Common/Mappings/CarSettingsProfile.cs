
using AutoMapper;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Car;
using BenzenyMain.Domain.Entity.Dto.Tag;

namespace BenzenyMain.Application.Common.Mappings
{
    public class CarSettingsProfile : Profile
    {
        public CarSettingsProfile()
        {
            CreateMap<CarBrand, CarBrandDto>().ReverseMap();
            CreateMap<CarModel, CarModelDto>().ReverseMap();
            CreateMap<PlateType, PlateTypeDto>().ReverseMap();
            CreateMap<CarType, CarTypeDto>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();
        }
    }
}
