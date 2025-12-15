
using AutoMapper;
using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto.Identity;

namespace Benzeny.Application.Common.Mappings
{
    public class RoleProfile :Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleDto, ApplicationRole>().ReverseMap();
        }
    }
}
