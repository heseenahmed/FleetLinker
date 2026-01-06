using FleetLinker.Application.Command.Equipment;
using FleetLinker.Application.Queries.Equipment;
using FleetLinker.Application.DTOs.Equipment;
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
    public class EquipmentController : ApiController
    {
        public EquipmentController(IMediator mediator, UserManager<ApplicationUser> userManager, IAppLocalizer localizer) 
            : base(mediator, userManager, localizer)
        {
        }

        [HttpPost("CreateEquipment")]
        public async Task<IActionResult> Create([FromBody] CreateEquipmentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new CreateEquipmentCommand(dto, userId!));
            return Ok(result);
        }

        [HttpGet("GetAllEquipments")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllEquipmentsQuery());
            return Ok(result);
        }

        [HttpGet("GetEquipmentById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetEquipmentByIdQuery(id));
            return Ok(result);
        }

        [HttpPut("UpdateEquipment")]
        public async Task<IActionResult> Update([FromBody] UpdateEquipmentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new UpdateEquipmentCommand(dto, userId!));
            return Ok(result);
        }

        [HttpDelete("DeleteEquipment/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new DeleteEquipmentCommand(id, userId!));
            return Ok(result);
        }

        [HttpGet("DownloadTemplate")]
        public async Task<IActionResult> DownloadTemplate()
        {
            var result = await _mediator.Send(new DownloadEquipmentTemplateQuery());
            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EquipmentTemplate.xlsx");
        }

        [HttpPost("UploadExcel")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            using (var stream = file.OpenReadStream())
            {
                var result = await _mediator.Send(new UploadEquipmentExcelCommand(stream, userId!));
                return Ok(result);
            }
        }
    }
}
