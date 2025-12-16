using FleetLinker.Application.Command.User;
using FleetLinker.Application.Queries.User;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Entity.Dto;
using FleetLinker.Domain.Entity.Dto.Identity;
using FleetLinker.Domain.Entity.Dto.User;
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
    public class UserManagerController : ApiController
    {
        #region Constructor
        public UserManagerController(ISender mediator, UserManager<ApplicationUser> userManager)
            : base(mediator, userManager)
        {
        }
        #endregion
        #region User Registration
        [AllowAnonymous]
        [HttpPost("RegisterNewUser")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto model, CancellationToken ct)
        {
            if (model == null)
                throw new ValidationException("Body is required.");
            var performedBy = User?.Identity?.Name ?? "System";
            
            var success = await _mediator.Send(new RegisterCommand(model, performedBy), ct);
            return Ok(APIResponse<bool>.Success(success, "User registered successfully."));
        }
        #endregion
        #region User Update
        [HttpPut("Update")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string id, [FromBody] UserForUpdateDto model, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(id) || model == null || id != model.Id)
                throw new ArgumentException("User data is missing or inconsistent.");
            var performedBy = User?.Identity?.Name ?? "System";
            var result = await _mediator.Send(new UpdateUserAsyncCommand(model, performedBy), ct);
            return Ok(APIResponse<bool>.Success(result, "User updated successfully."));
        }
        #endregion
        #region User Activation
        [HttpPost("SwitchUserActive/{id}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SwitchUserActive(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid user ID.");
            var performedBy = User?.Identity?.Name ?? "System";
            var result = await _mediator.Send(new SwitchUserActiveCommand(id, performedBy), ct);
            return Ok(APIResponse<object>.Success(null, result ? "User activated successfully." : "User deactivated."));
        }
        #endregion
        #region User Queries
        [HttpGet("GetById/{id}")]
        [ProducesResponseType(typeof(APIResponse<UserForListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Invalid user ID.");
            var userInfo = await _mediator.Send(new GetUserById(id), ct);
            return Ok(APIResponse<UserForListDto>.Success(userInfo, "User information retrieved successfully."));
        }
        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(APIResponse<List<UserForListDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var users = await _mediator.Send(new GetAllUser(), ct);
            return Ok(APIResponse<List<UserForListDto>>.Success(users, "Users information retrieved successfully."));
        }
        #endregion
        #region User Deletion
        [HttpDelete("DeleteUser/{userId}")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(string userId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid user ID.");
            var performedBy = User?.Identity?.Name ?? "System";
            var result = await _mediator.Send(new DeleteUserCommand(userId, performedBy), ct);
            if (!result)
                throw new KeyNotFoundException("User not found or already deleted.");
            return Ok(APIResponse<bool>.Success(true, "User deleted successfully."));
        }
        #endregion
        #region Role Management
        [HttpPut("UpdateUserRoles")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesRequest body, CancellationToken ct)
        {
            var performedBy = User?.Identity?.Name ?? "System";
            var result = await _mediator.Send(
                new UpdateUserRolesCommand(body.UserId, body.Roles, performedBy), ct);
            return Ok(APIResponse<UpdateUserRolesResult>
                .Success(result, "User roles updated"));
        }
        #endregion
    }
}
