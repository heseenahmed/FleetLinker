using Benzeny.Application.Command.Core;
using Benzeny.Domain.Entity.Dto.Identity;
using MediatR;

namespace Benzeny.Application.Command.Companies
{
    public class UserCommand : Commands
    {
        public string Id { get; set; }
        public RoleDto Role { get; set; }
    }
    public class AddRoleCommand : UserCommand
    {
        public RoleDto Role { get; set; }
        public AddRoleCommand(RoleDto role)
        {
            Role = role;
        }
    }
    public class DeleteRoleCommand : UserCommand
    {
        public string RoleName { get; set; }
        public DeleteRoleCommand(string RoleName)
        {
            this.RoleName = RoleName;
        }
    }
    public class UpdateRoleCommand : UserCommand
    {
        public RoleDto Role { get; set; }
        public UpdateRoleCommand(RoleDto role)
        {
            Role = role;
        }
    }
}
