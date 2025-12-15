using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Application.Queries.City;
using BenzenyMain.Domain.Entity.Dto.City;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BenzenyMain.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class CityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/City/GetCitiesByRegionId/{regionId}
        [HttpGet("GetCitiesByRegionId/{regionId}")]
        [ProducesResponseType(typeof(APIResponse<List<CityDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByRegion(Guid regionId, CancellationToken ct)
        {
            if (regionId == Guid.Empty)
                throw new ArgumentException("Invalid region ID.");

            var result = await _mediator.Send(new GetCitiesByRegionQuery(regionId), ct);
            return Ok(APIResponse<List<CityDto>>.Success(result, "Cities retrieved successfully"));
        }
    }
}
