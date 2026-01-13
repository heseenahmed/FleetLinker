using FleetLinker.API.Controllers;
using FleetLinker.Application.Command.Order;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
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
    public class OrderController : ApiController
    {
        public OrderController(IMediator mediator, UserManager<ApplicationUser> userManager, IAppLocalizer localizer)
            : base(mediator, userManager, localizer)
        {
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new CheckoutCommand(userId!));
            return Ok(result);
        }

        [HttpGet("GetMyOrders")]
        public async Task<IActionResult> GetOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetOrderHistoryQuery(userId!));
            return Ok(result);
        }

        [HttpGet("GetOrderDetails/{id}")]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetOrderByIdQuery(id, userId!));
            return Ok(result);
        }
    }
}
