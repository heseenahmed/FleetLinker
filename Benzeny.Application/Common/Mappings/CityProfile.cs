

using AutoMapper;
using BenzenyMain.Domain.Entity.Dto.City;

namespace BenzenyMain.Application.Common.Mappings
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<Domain.Entity.City, CityDto>().ReverseMap();
        }
    }
}
