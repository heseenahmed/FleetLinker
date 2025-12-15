using Benzeny.Application.Command.Companies;
using Benzeny.Application.Queries.Roles;
using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto;
using Benzeny.Domain.Entity.Dto.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Benzeny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class RolesController : ApiController
    {
        public RolesController(ISender mediator, UserManager<ApplicationUser> userManager)
            : base(mediator, userManager) { }

        // GET: api/Roles/GetAllRoles
        [HttpGet("GetAllRoles")]
        [ProducesResponseType(typeof(APIResponse<List<ApplicationRole>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllRoles(CancellationToken ct)
        {
            var roles = await _mediator.Send(new GetRoleList(), ct);
            return Ok(APIResponse<List<ApplicationRole>>.Success(roles.ToList(), "Roles retrieved successfully."));
        }

        // POST: api/Roles/AddNewRole
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

        // DELETE: api/Roles/DeleteRole/roleName
        // Note: route is a fixed segment; roleName is expected from the query string (kept exactly as provided).
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
    }
}
