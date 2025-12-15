using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto.Identity;
using MediatR;

namespace Benzeny.Application.Queries.Roles
{
    public record GetRoleList : IRequest<IEnumerable<ApplicationRole>>;
    
}