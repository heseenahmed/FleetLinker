
using AutoMapper;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Car;

namespace BenzenyMain.Application.Common.Mappings
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {
            // Map from CarForCreateDto → Car
            CreateMap<CarForCreateDto, Car>();

            // Map from UpdateCarDto → Car
            CreateMap<UpdateCarDto, Car>();

            // Map from Car → CarForGet
            CreateMap<Car, CarForGet>()
                .ForMember(dest => dest.CarModel, opt => opt.MapFrom(src => src.CarModel != null ? src.CarModel.Title : string.Empty))
                .ForMember(dest => dest.CarBrand, opt => opt.MapFrom(src => src.CarBrand != null ? src.CarBrand.Title : string.Empty))
                .ForMember(dest => dest.CarType, opt => opt.MapFrom(src => src.CarType != null ? src.CarType.Title : string.Empty))
                .ForMember(dest => dest.PlateType, opt => opt.MapFrom(src => src.PlateType != null ? src.PlateType.Title : string.Empty))
                .ForMember(dest => dest.PetrolType, opt => opt.MapFrom(src => src.Petroltype))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color ?? string.Empty))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                
                .ForMember(dest => dest.DriversName, opt => opt.MapFrom(src =>
                                src.CarDrivers
                                    .Where(d => d.Driver != null && d.Driver.User != null)
                                    .Select(d => d.Driver.User.FullName)
                                    .ToList()
                ));
            CreateMap<Car, CarForListDto>()
                .ForMember(dest => dest.CarModel, opt => opt.MapFrom(src => src.CarModel != null ? src.CarModel.Title : string.Empty))
                .ForMember(dest => dest.CarBrand, opt => opt.MapFrom(src => src.CarBrand != null ? src.CarBrand.Title : string.Empty))
                .ForMember(dest => dest.PlateType, opt => opt.MapFrom(src => src.PlateType != null ? src.PlateType.Title : string.Empty))
                .ForMember(dest => dest.CarType, opt => opt.MapFrom(src => src.CarType != null ? src.CarType.Title : string.Empty))
                .ForMember(dest => dest.CarModelId, opt => opt.MapFrom(src => src.CarModelId))
                .ForMember(dest => dest.CarBrandId, opt => opt.MapFrom(src => src.CarBrandId)) // تأكد أنك عدلت في الـ DTO
                .ForMember(dest => dest.PlateTypeId, opt => opt.MapFrom(src => src.PlateTypeId))
                .ForMember(dest => dest.CarTypeId, opt => opt.MapFrom(src => src.CarTypeId))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.BranchId))
                .ForMember(dest => dest.BranchTitle, opt => opt.MapFrom(src => src.Branch.Company.Name + " - " + src.Branch.Address ?? "NA"))
                .ForMember(dest => dest.DriversName, opt => opt.MapFrom(src =>
                    src.CarDrivers
                        .Where(cd => cd.Driver != null && cd.Driver.User != null)
                        .Select(cd => cd.Driver.User.FullName)
                        .ToList()
                ))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.CarModel != null ? src.CarModel.Title : string.Empty))
                .ForMember(dest => dest.CarNumber, opt => opt.MapFrom(src => src.CarNumber))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.CardNum, opt => opt.MapFrom(src => src.CardNum))
                .ForMember(dest => dest.LicenseDate, opt => opt.MapFrom(src => src.LicenseDate))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

        }
    }
}
