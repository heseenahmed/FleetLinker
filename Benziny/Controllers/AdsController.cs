using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Application.Command.Ads;
using BenzenyMain.Application.Queries.Ads;
using BenzenyMain.Domain.Entity.Dto.Ads;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BenzenyMain.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class AdsController : ControllerBase
    {
        
        //
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdsController(IMediator mediator , IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        // POST: api/Ads/CreateAds
        [HttpPost("CreateAds")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromForm] CreateAdsDto dto, CancellationToken ct)
        {
            var CreatedBy = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";

            var result = await _mediator.Send(new CreateAdsCommand(dto , CreatedBy), ct);
            return Ok(APIResponse<bool>.Success(result, "Ads Created Successfully"));
        }

        // PUT: api/Ads/UpdateAds/{id}
        [HttpPut("UpdateAds/{id}")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateAdsDto dto, CancellationToken ct)
        {
            var UpdtedBy = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";

            var result = await _mediator.Send(new UpdateAdvertisementCommand(dto, id , UpdtedBy), ct);
            return Ok(APIResponse<bool>.Success(result, "Ads Updated Successfully"));
        }

        // DELETE: api/Ads/DeleteAds/{id}
        [HttpDelete("DeleteAds/{id}")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new HardDeleteAdsCommand(id), ct);
            return Ok(APIResponse<bool>.Success(result, "Ads Deleted Successfully"));
        }

        // GET: api/Ads/GetSystemAds
        [HttpGet("GetSystemAds")]
        [ProducesResponseType(typeof(APIResponse<AdsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSystemAds(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetSystemAdsQuery(), ct); // returns AdsDto
            return Ok(APIResponse<AdsDto>.Success(result, "Ads Fetched Successfully"));
        }

        // GET: api/Ads/GetAllActiveAds
        [HttpGet("GetAllActiveAds")]
        [ProducesResponseType(typeof(APIResponse<AdsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllActive(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAllActiveAdsQuery(), ct); // returns AdsDto
            return Ok(APIResponse<AdsDto>.Success(result, "Ads Fetched Successfully"));
        }

        // GET: api/Ads/GetAllMobileAds
        [HttpGet("GetAllMobileAds")]
        [ProducesResponseType(typeof(APIResponse<AdsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMobileAds(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMobileAdsQuery(), ct); // returns AdsDto
            return Ok(APIResponse<AdsDto>.Success(result, "Ads Fetched Successfully"));
        }

        // GET: api/Ads/GetAdsById/{id}
        [HttpGet("GetAdsById/{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<AdsForGetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAdsByIdQuery(id), ct); // returns AdsForGetDto
            return Ok(APIResponse<AdsForGetDto>.Success(result, "Ads Fetched Successfully"));
        }

        // GET: api/Ads/GetAllAds
        [HttpGet("GetAllAds")]
        [ProducesResponseType(typeof(APIResponse<AllAdsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAllAdsQuery(), ct); // returns AllAdsDto
            return Ok(APIResponse<AllAdsDto>.Success(result, "Ads Fetched Successfully"));
        }

        // PUT: api/Ads/SwitchActive/{id}
        [HttpPut("SwitchActive/{id}")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SwitchActive(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new SwitchActiveAdsCommand(id), ct);
            return Ok(APIResponse<bool>.Success(result, "Switch Active Successfully"));
        }
    }
}
