using FleetLinker.Application.Command.EquipmentSparePart;
using FleetLinker.Application.Queries.EquipmentSparePart;
using FleetLinker.Application.DTOs.EquipmentSparePart;
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
    public class EquipmentSparePartController : ApiController
    {
        public EquipmentSparePartController(IMediator mediator, UserManager<ApplicationUser> userManager, IAppLocalizer localizer) 
            : base(mediator, userManager, localizer)
        {
        }

        [HttpPost("CreateSparePart")]
        public async Task<IActionResult> Create([FromForm] CreateEquipmentSparePartDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new CreateEquipmentSparePartCommand(dto, userId!));
            return Ok(result);
        }

        [HttpGet("GetAllSpareParts")]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var result = await _mediator.Send(new GetAllEquipmentSparePartsQuery(search));
            return Ok(result);
        }

        [HttpGet("GetSparePartById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetEquipmentSparePartByIdQuery(id));
            return Ok(result);
        }

        [HttpPut("UpdateSparePart")]
        public async Task<IActionResult> Update([FromForm] UpdateEquipmentSparePartDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new UpdateEquipmentSparePartCommand(dto, userId!));
            return Ok(result);
        }

        [HttpDelete("DeleteSparePart/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new DeleteEquipmentSparePartCommand(id, userId!));
            return Ok(result);
        }

        [HttpGet("DownloadExcelTemplate")]
        public async Task<IActionResult> DownloadExcelTemplate()
        {
            var result = await _mediator.Send(new DownloadSparePartTemplateQuery());
            if (result.Result == "Success")
            {
                return File(result.Data!, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SparePartTemplate.xlsx");
            }
            return BadRequest(result);
        }

        [HttpPost("UploadExcelFile")]
        public async Task<IActionResult> UploadExcelFile(IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new BatchUploadSparePartsCommand(file, userId!));
            return Ok(result);
        }
    }
}
