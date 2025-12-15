using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Application.Command.Region;
using BenzenyMain.Domain.Entity.Dto.Region;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using static BenzenyMain.Application.Queries.Region.RegionQuery;

namespace BenzenyMain.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class RegionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Region/GetAllRegions
        [HttpGet("GetAllRegions")]
        [ProducesResponseType(typeof(APIResponse<PaginatedResult<RegionForListDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetRegionList(pageNumber, pageSize, searchTerm), ct);
            return Ok(APIResponse<PaginatedResult<RegionForListDto>>.Success(result, "Regions retrieved successfully"));
        }

        // GET: api/Region/GetRegionById/{id}
        [HttpGet("GetRegionById/{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<RegionForListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid region ID.");

            var region = await _mediator.Send(new GetRegionById(id), ct);
            return Ok(APIResponse<RegionForListDto>.Success(region, "Region found"));
        }

        // POST: api/Region/CreateRegion
        [HttpPost("CreateRegion")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] RegionForCreateDto model, CancellationToken ct)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Title))
                throw new ValidationException("Title is required.");

            model.CreatedBy = User.FindFirst("uid")?.Value ?? User.FindFirst("sub")?.Value ?? "System";

            var result = await _mediator.Send(new CreateRegionCommand(model), ct);
            return Ok(APIResponse<bool>.Success(result, "Region created successfully"));
        }

        // PUT: api/Region/UpdateRegion/{id}
        [HttpPut("UpdateRegion/{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] RegionForUpdateDto model, CancellationToken ct)
        {
            if (id == Guid.Empty || model == null)
                throw new ArgumentException("Invalid region ID.");

            if (id != model.Id)
                throw new ArgumentException("The ID in the URL does not match the ID in the request body.");

            model.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("uid")?.Value
                              ?? User.FindFirst("sub")?.Value
                              ?? "System";

            var result = await _mediator.Send(new UpdateRegionCommand(id, model), ct);
            return Ok(APIResponse<bool>.Success(result, "Region updated successfully"));
        }

        // DELETE: api/Region/deleteRegion/{id}
        [HttpDelete("deleteRegion/{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid region ID.");

            var result = await _mediator.Send(new DeleteRegionCommand(id,
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("uid")?.Value
                ?? User.FindFirst("sub")?.Value
                ?? "System"), ct);

            return Ok(APIResponse<bool>.Success(result, "Region deleted successfully"));
        }
    }
}
