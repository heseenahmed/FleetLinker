using FleetLinker.Application.Command.SparePartOffer;
using FleetLinker.Application.Queries.SparePartOffer;
using FleetLinker.Application.DTOs.SparePartOffer;
using FleetLinker.Application.DTOs;
using FleetLinker.Domain.Entity;
using FleetLinker.Application.Common.Localization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FleetLinker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SparePartOfferController : ApiController
    {
        public SparePartOfferController(IMediator mediator, UserManager<ApplicationUser> userManager, IAppLocalizer localizer) 
            : base(mediator, userManager, localizer)
        {
        }

        [HttpPost("CreateOffer")]
        public async Task<IActionResult> Create([FromBody] CreateSparePartOfferDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new CreateSparePartOfferCommand(dto, userId!));
            return Ok(result);
        }

        [HttpPost("RespondToOffer")]
        public async Task<IActionResult> Respond([FromBody] RespondToSparePartOfferDto dto)
        {
            if (!User.IsInRole("Supplier"))
            {
                return Ok(APIResponse<bool>.Fail(403, message: _localizer[LocalizationMessages.UnauthorizedSupplierAccess]));
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new RespondToSparePartOfferCommand(dto, userId!));
            return Ok(result);
        }

        [HttpGet("GetSupplierOffers")]
        public async Task<IActionResult> GetSupplierOffers()
        {
            if (!User.IsInRole("Supplier"))
            {
                return Ok(APIResponse<List<SparePartOfferDto>>.Fail(403, message: _localizer[LocalizationMessages.UnauthorizedSupplierAccess]));
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetSupplierSparePartOffersQuery(userId!));
            return Ok(result);
        }
    }
}
