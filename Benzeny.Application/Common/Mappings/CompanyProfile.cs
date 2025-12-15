
using AutoMapper;
using Benzeny.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Company;

namespace BenzenyMain.Application.Common.Mappings
{
    public class CompanyProfile:Profile
    {
        public CompanyProfile()
        {
            CreateMap<Domain.Entity.Company, CompanyForCreateDto>().ReverseMap();
            CreateMap<Domain.Entity.Company, CompanyForListDto>().ReverseMap();
            CreateMap<Domain.Entity.Company, CompanyDto>();
            CreateMap<Domain.Entity.Company, GetCompanyDetailsDto>();
    
        }
    }
}
