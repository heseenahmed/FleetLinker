using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Application.Command.Driver;
using BenzenyMain.Application.Contracts.Export;
using BenzenyMain.Application.Queries.Driver;
using BenzenyMain.Application.Queries.Tag;
using BenzenyMain.Domain.Entity.Dto.Driver;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BenzenyMain.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class DriversController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDriversTemplateBuilder _templateBuilder;
        public DriversController(IMediator mediator, IHttpContextAccessor httpContextAccessor , IDriversTemplateBuilder templateBuilder)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _templateBuilder = templateBuilder;
        }

        // POST: api/Drivers/CreateDriver
        [HttpPost("CreateDriver")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDriver([FromBody] DriverForCreateDto model, CancellationToken ct)
        {
            model.CreatedDate = DateTime.UtcNow;
            model.CreatedBy = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            var result = await _mediator.Send(new CreateDriverCommand(model), ct);
            return Ok(APIResponse<bool>.Success(result, "Driver created successfully."));
        }

        // GET: api/Drivers/GetAllDrivers
        [HttpGet("GetAllDrivers")]
        [ProducesResponseType(typeof(APIResponse<PaginatedResult<DriverForListDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllDrivers([FromQuery] string? searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than zero.");

            var result = await _mediator.Send(new GetAllDriversQuery(pageNumber, pageSize, searchTerm), ct);
            return Ok(APIResponse<PaginatedResult<DriverForListDto>>.Success(result, "Drivers retrieved successfully."));
        }

        // GET: api/Drivers/GetDriverById/{id}
        [HttpGet("GetDriverById/{id}")]
        [ProducesResponseType(typeof(APIResponse<DriverForGetIdDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDriverById(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid driver ID.");

            var driver = await _mediator.Send(new GetDriverByIdQuery(id), ct);
            return Ok(APIResponse<DriverForGetIdDto>.Success(driver, "Driver retrieved successfully."));
        }

        // GET: api/Drivers/GetDriversInCompany/{CompanyId}
        [HttpGet("GetDriversInCompany/{CompanyId}")]
        [ProducesResponseType(typeof(APIResponse<DriverStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDriversInCompany(Guid CompanyId, CancellationToken ct)
        {
            if (CompanyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var driverStatus = await _mediator.Send(new GetDriversStatusInCompany(CompanyId), ct);
            return Ok(APIResponse<DriverStatusDto>.Success(driverStatus, "Driver status retrieved successfully."));
        }

        // GET: api/Drivers/GetDriversInBranch/{id}
        [HttpGet("GetDriversInBranch/{id}")]
        [ProducesResponseType(typeof(APIResponse<PaginatedResult<DriverForListDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDriversInBranch(Guid id, [FromQuery] string? searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");

            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than zero.");

            var result = await _mediator.Send(new GetDriverInBranchQuery(id, pageNumber, pageSize, searchTerm), ct);
            return Ok(APIResponse<PaginatedResult<DriverForListDto>>.Success(result, "Drivers retrieved successfully."));
        }

        // DELETE: api/Drivers/DeleteDriver/{id}
        [HttpDelete("DeleteDriver/{id}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDriver(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid driver ID.");

            await _mediator.Send(new DeleteDriverCommand(id), ct);
            return Ok(APIResponse<object>.Success(null, "Driver deleted successfully."));
        }

        // POST: api/Drivers/AssignDriverToCar
        [HttpPost("AssignDriverToCar")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignDriverToCar([FromBody] CarDriverDto carDriver, CancellationToken ct)
        {
            if (carDriver == null)
                throw new ValidationException("Body is required.");

            if (!ModelState.IsValid)
                throw new ValidationException(string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            var result = await _mediator.Send(new AssignDriverToCarCommand(carDriver), ct);
            return Ok(APIResponse<bool>.Success(result, "Driver assigned to car successfully."));
        }

        // POST: api/Drivers/UnassignDriverFromCar
        [HttpPost("UnassignDriverFromCar")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UnassignDriverFromCar([FromBody] CarDriverDto carDriver, CancellationToken ct)
        {
            if (carDriver == null)
                throw new ValidationException("Body is required.");

            if (!ModelState.IsValid)
                throw new ValidationException(string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            var result = await _mediator.Send(new UnAssignDriverToCarCommand(carDriver), ct);
            return Ok(APIResponse<bool>.Success(result, "Driver unassigned from car successfully."));
        }

        // POST: api/Drivers/import
        [HttpPost("importDriversExcel")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImportDrivers([FromForm] ExcelDto file, [FromForm] Guid branchId, CancellationToken ct)
        {
            if (file?.File == null || file.File.Length == 0)
                throw new ValidationException("Please upload a valid Excel file.");

            if (branchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            var result = await _mediator.Send(new ImportDriversFromExcelCommand(file.File, userId, branchId), ct);
            return Ok(APIResponse<bool>.Success(result, "Drivers imported successfully."));
        }

        // POST: api/Drivers/DriverSwitchActive/{id}
        [HttpPost("DriverSwitchActive/{id}")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DriverSwitchActive(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid driver ID.");

            var result = await _mediator.Send(new DriverSwitchActiveCommand(id), ct);
            return Ok(APIResponse<bool>.Success(result, "Driver status updated successfully."));
        }

        // POST: api/Drivers/assign-funds
        [HttpPost("assign-funds")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignFundsToDrivers([FromBody] AssignDriverFundsRequestDto request, CancellationToken ct)
        {
            if (request == null || request.DriversIds == null || !request.DriversIds.Any())
                throw new ValidationException("DriversIds list cannot be empty.");

            var result = await _mediator.Send(new AssignDriverFundsCommand(request), ct);
            return Ok(APIResponse<bool>.Success(result, "Funds assigned successfully."));
        }
        [HttpGet("import/DriverTemplateExcel")]
        public async Task<IActionResult> DownloadDriversTemplate(CancellationToken ct)
        {
            var tags = await _mediator.Send(new GetAllTagsForTemplateQuery(), ct);
            var bytes = _templateBuilder.Build(tags);
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"drivers_template_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx");
        }
    }
}
