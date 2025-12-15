
using AutoMapper;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Branch;

namespace BenzenyMain.Application.Common.Mappings
{
    public class BranchProfile : Profile
    {
        public BranchProfile()
        {
            CreateMap<BranchForCreateDto, Branch>()
           .ForMember(dest => dest.Id, opt => opt.Ignore())
           .ForMember(dest => dest.Balance, opt => opt.Ignore())
           .ForMember(dest => dest.IBAN, opt => opt.Ignore())
           .ForMember(dest => dest.IsActive, opt => opt.Ignore())
           .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
           .ReverseMap();
            CreateMap<Branch, BranchForListDto>()
            .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.Company.Name))
            .ForMember(d => d.CityTitle, opt => opt.MapFrom(s => s.City.Name))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber))
            .ForMember(d => d.IBAN, opt => opt.MapFrom(s => s.IBAN ?? "N/A")).ReverseMap();
        }
    }
}
