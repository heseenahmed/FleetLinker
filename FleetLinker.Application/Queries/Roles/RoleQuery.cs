using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Entity.Dto.Identity;
using MediatR;
namespace FleetLinker.Application.Queries.Roles
{
    public record GetRoleList : IRequest<IEnumerable<ApplicationRole>>;
}