using FleetLinker.Application.Command.Companies;
using FleetLinker.Application.Queries.Roles;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Entity.Dto;
using FleetLinker.Domain.Entity.Dto.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace FleetLinker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class RolesController : ApiController
    {
        #region Constructor
        public RolesController(ISender mediator, UserManager<ApplicationUser> userManager)
            : base(mediator, userManager) { }
        #endregion
        #region Get Roles
        [HttpGet("GetAllRoles")]
        [ProducesResponseType(typeof(APIResponse<List<ApplicationRole>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllRoles(CancellationToken ct)
        {
            var roles = await _mediator.Send(new GetRoleList(), ct);
            return Ok(APIResponse<List<ApplicationRole>>.Success(roles.ToList(), "Roles retrieved successfully."));
        }
        #endregion
        #region Add Role
        [HttpPost("AddNewRole")]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddRole([FromBody] RoleDto role, CancellationToken ct)
        {
            if (role == null || string.IsNullOrWhiteSpace(role.Name))
                throw new ValidationException("Role name is required.");
            var result = await _mediator.Send(new AddRoleCommand(role), ct);
            if (!result)
                throw new ApplicationException("Error when adding new role.");
            return Ok(APIResponse<string>.Success("Role Added Successfully!"));
        }
        #endregion
        #region Delete Role
        [HttpDelete("DeleteRole/roleName")]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteByName(string roleName, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role Name cannot be null.");
            var result = await _mediator.Send(new DeleteRoleCommand(roleName), ct);
            if (!result)
                throw new KeyNotFoundException($"Role {roleName} not found.");
            return Ok(APIResponse<string>.Success("Role Deleted Successfully!"));
        }
        #endregion
    }
}
