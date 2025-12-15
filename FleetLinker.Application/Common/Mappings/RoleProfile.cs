using AutoMapper;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Entity.Dto.Identity;
namespace FleetLinker.Application.Common.Mappings
{
    public class RoleProfile :Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleDto, ApplicationRole>().ReverseMap();
        }
    }
}
