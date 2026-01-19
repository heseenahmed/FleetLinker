using FleetLinker.API.Controllers;
using FleetLinker.Application.Command.Equipment.EquipmentRequest;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.Equipment;
using FleetLinker.Application.Queries.Equipment.EquipmentRequest;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Enums;
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
    public class EquipmentMaintenanceRequestController : ApiController
    {
        public EquipmentMaintenanceRequestController(IMediator mediator, UserManager<ApplicationUser> userManager, IAppLocalizer localizer)
            : base(mediator, userManager, localizer)
        {
        }

        [HttpPost("Create")]
        [Authorize(Roles = "mechanical")]
        public async Task<IActionResult> Create([FromBody] CreateMaintenanceRequestDto maintenanceDto)
        {
            var dto = new CreateEquipmentRequestDto
            {
                EquipmentId = maintenanceDto.EquipmentId,
                MaintenanceDescription = maintenanceDto.MaintenanceDescription,
                RequestType = EquipmentRequestType.Maintenance
            };
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new CreateEquipmentRequestCommand(dto, userId!));
            return Ok(result);
        }

        [HttpPost("Respond")]
        public async Task<IActionResult> Respond([FromBody] RespondToMaintenanceRequestDto maintenanceDto)
        {
            var dto = new RespondToEquipmentRequestDto
            {
                RequestId = maintenanceDto.RequestId,
                MaintenanceResponse = maintenanceDto.MaintenanceResponse,
                Status = EquipmentRequestStatus.Responded
            };
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new RespondToEquipmentRequestCommand(dto, userId!));
            return Ok(result);
        }

        [HttpGet("SentByMechanical")]
        [Authorize(Roles = "mechanical")]
        public async Task<IActionResult> GetSentByMechanical()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetMechanicalMaintenanceRequestsQuery(userId!));
            return Ok(result);
        }

        [HttpGet("ReceivedByOwner")]
        public async Task<IActionResult> GetReceivedByOwner()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetOwnerMaintenanceRequestsQuery(userId!));
            return Ok(result);
        }
    }
}
