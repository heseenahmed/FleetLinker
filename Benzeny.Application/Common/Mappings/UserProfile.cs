
using AutoMapper;
using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto.Identity;
using BenzenyMain.Domain.Entity.Dto.Company;

namespace Benzeny.Application.Common.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {

            CreateMap<ApplicationUser, UserForListDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Active))
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src =>
                    src.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : null).Where(name => name != null).ToList()))
                .ForMember(dest => dest.BranchId, opt => opt.Ignore())
                .ForMember(dest => dest.BranchName, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    var userBranch = src.UserBranches.FirstOrDefault();
                    if (userBranch != null && userBranch.Branch != null)
                    {
                        dest.BranchId = userBranch.BranchId;
                        dest.BranchName = userBranch.Branch.Address;
                    }
                    else
                    {
                        dest.BranchId = null;
                        dest.BranchName = null;
                    }
                })
                .ReverseMap();

            CreateMap<ApplicationUser, GetUserDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.PhoneNumber));


        }
    }
}