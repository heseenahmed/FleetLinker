
using AutoMapper;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Driver;

namespace BenzenyMain.Application.Common.Mappings
{
    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<Driver, DriverForListDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Company.Name + src.Branch.Address??"NA"))
                .ForMember(dest => dest.TagName, opt => opt.MapFrom(src => src.Tag!.Title))
                .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.CarDrivers.FirstOrDefault()!.CarId))
                .ForMember(dest => dest.CarPlate, opt => opt.MapFrom(src => src.CarDrivers.FirstOrDefault()!.Car.PlateType!.Title))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.ConsumptionType, opt => opt.MapFrom(src => src.FundingAssignments.FirstOrDefault()!.LimitType.ToString()));

            // DriverForCreateDto → Driver
            CreateMap<DriverForCreateDto, Driver>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // لأنه بيتعمل داخل الـ Handler بعد إنشاء المستخدم
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.branchId))
                .ForMember(dest => dest.License, opt => opt.MapFrom(src => src.License))
                .ForMember(dest => dest.LicenseDegree, opt => opt.MapFrom(src => src.LicenseDegree))
                .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.TagId))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));
        }
    }
    
}
