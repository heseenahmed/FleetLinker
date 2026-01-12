using FleetLinker.API.Controllers;
using FleetLinker.Application.Command.Equipment.EquipmentRequest;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.Equipment;
using FleetLinker.Application.Queries.Equipment.EquipmentRequest;
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
    public class EquipmentRequestController : ApiController
    {
        public EquipmentRequestController(IMediator mediator, UserManager<ApplicationUser> userManager, IAppLocalizer localizer)
            : base(mediator, userManager, localizer)
        {
        }

        [HttpPost("CreateRequest")]
        public async Task<IActionResult> Create([FromBody] CreateEquipmentRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new CreateEquipmentRequestCommand(dto, userId!));
            return Ok(result);
        }

        [HttpPost("RespondToRequest")]
        public async Task<IActionResult> Respond([FromBody] RespondToEquipmentRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new RespondToEquipmentRequestCommand(dto, userId!));
            return Ok(result);
        }

        [HttpGet("GetMyIncomingRequests")]
        public async Task<IActionResult> GetIncoming()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetOwnerEquipmentRequestsQuery(userId!));
            return Ok(result);
        }

        [HttpGet("GetMyOutgoingRequests")]
        public async Task<IActionResult> GetOutgoing()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetRequesterEquipmentRequestsQuery(userId!));
            return Ok(result);
        }
    }
}
