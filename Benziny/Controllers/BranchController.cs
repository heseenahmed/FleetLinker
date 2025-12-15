using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Application.Command.Branch;
using BenzenyMain.Application.Queries.Branch;
using BenzenyMain.Domain.Entity.Dto.Branch;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BenzenyMain.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class BranchController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BranchController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        // PUT: api/Branch/UpdateBranch/{id}
        [HttpPut("UpdateBranch/{id}")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBranch([FromBody] BranchForUpdateDto model, Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");

            model.UpdatedBy = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";

            var result = await _mediator.Send(new UpdateBranchCommand(id, model), ct);
            return Ok(APIResponse<bool>.Success(result, "Branch updated successfully"));
        }

        // POST: api/Branch/CreateBranch
        [HttpPost("CreateBranch")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromForm] BranchForCreateDto model, CancellationToken ct)
        {
            // If you keep [ApiController], consider suppressing automatic 400s in Program.cs,
            // otherwise this manual throw ensures middleware formats the response.
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                throw new ValidationException(string.Join(" | ", errors));
            }

            model.CreatedBy = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";

            var response = await _mediator.Send(new CreateBranchCommand(model), ct); // returns APIResponse with ApiStatusCode
            return StatusCode(response.ApiStatusCode, response);
        }

        // GET: api/Branch/GetAllBranches
        [HttpGet("GetAllBranches")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllBranches(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = "",
            CancellationToken ct = default)
        {
            var response = await _mediator.Send(new GetBranchList(pageNumber, pageSize, searchTerm), ct); // returns APIResponse
            return StatusCode(response.ApiStatusCode, response);
        }

        // GET: api/Branch/GetById/{id}
        [HttpGet("GetById/{id}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBranchById(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");

            var response = await _mediator.Send(new GetBranchById(id), ct); // returns APIResponse
            return StatusCode(response.ApiStatusCode, response);
        }

        // GET: api/Branch/GetAllBranchesInCompany/{companyId}
        [HttpGet("GetAllBranchesInCompany/{companyId}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllBranchesInCompany(
            Guid companyId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = "",
            CancellationToken ct = default)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var response = await _mediator.Send(new GetBranchsInCompany(companyId, pageNumber, pageSize, searchTerm), ct); // returns APIResponse
            return StatusCode(response.ApiStatusCode, response);
        }

        // POST: api/Branch/SwitchActive/{branchId}
        [HttpPost("SwitchActive/{branchId}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SwitchActive(Guid branchId, CancellationToken ct)
        {
            if (branchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");

            var response = await _mediator.Send(new SwitchActiveCommand(branchId), ct); // returns APIResponse
            return StatusCode(response.ApiStatusCode, response);
        }

        // DELETE: api/Branch/DeleteBranch/{id}
        [HttpDelete("DeleteBranch/{id}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBranch(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");
            var response = await _mediator.Send(new DeleteBranchCommand(id), ct); // returns APIResponse
            return StatusCode(response.ApiStatusCode, response);
        }

        // POST: api/Branch/AssignUserToBranch
        [HttpPost("AssignUserToBranch")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignUserToBranch([FromBody] UserAndBranchAssignation request, CancellationToken ct)
        {
            var result = await _mediator.Send(new AssignUserToBranchCommand(request.UserId, request.BranchId), ct);

            if (!result)
                throw new ApplicationException("User already assigned or branch not found.");

            return Ok(APIResponse<bool>.Success(true, "User assigned to branch successfully"));
        }

        // DELETE: api/Branch/UnassignUserFromBranch
        [HttpDelete("UnassignUserFromBranch")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UnassignUserFromBranch([FromBody] UserAndBranchAssignation request, CancellationToken ct)
        {
            var result = await _mediator.Send(new UnassignUserFromBranchCommand(request.UserId, request.BranchId), ct);

            if (!result)
                throw new KeyNotFoundException("User is not assigned to this branch.");

            return Ok(APIResponse<bool>.Success(true, "User unassigned from branch successfully"));
        }
    }
}
