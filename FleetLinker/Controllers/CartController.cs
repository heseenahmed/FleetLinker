using FleetLinker.API.Controllers;
using FleetLinker.Application.Command.Cart;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.CartAndOrder;
using FleetLinker.Application.Queries.CartAndOrder;
using FleetLinker.Domain.Entity;
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
    public class CartController : ApiController
    {
        public CartController(IMediator mediator, UserManager<ApplicationUser> userManager, IAppLocalizer localizer)
            : base(mediator, userManager, localizer)
        {
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> Add([FromBody] AddToCartDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new AddToCartCommand(dto, userId!));
            return Ok(result);
        }

        [HttpDelete("RemoveFromCart/{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new RemoveFromCartCommand(id, userId!));
            return Ok(result);
        }

        [HttpGet("GetMyCart")]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetCartQuery(userId!));
            return Ok(result);
        }

        [HttpDelete("ClearCart")]
        public async Task<IActionResult> Clear()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new ClearCartCommand(userId!));
            return Ok(result);
        }
    }
}
