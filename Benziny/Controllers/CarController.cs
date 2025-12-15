using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Application.Command.Car;
using BenzenyMain.Application.Command.Driver;
using BenzenyMain.Application.Queries.Car;
using BenzenyMain.Domain.Entity.Dto.Car;
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
    public class CarController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarController(IMediator mediator , IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/Car/GetAllCars
        [HttpGet("GetAllCars")]
        [ProducesResponseType(typeof(APIResponse<PaginatedResult<CarForGet>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetAllCarsQuery(pageNumber, pageSize, search), ct);
            return Ok(APIResponse<PaginatedResult<CarForGet>>.Success(result, "Cars Fetched Successfully"));
        }

        // GET: api/Car/GetCarById/{id}
        [HttpGet("GetCarById/{id}")]
        [ProducesResponseType(typeof(APIResponse<CarForListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid car ID.");

            var result = await _mediator.Send(new GetCarByIdQuery(id), ct);
            return Ok(APIResponse<CarForListDto>.Success(result, "Car Fetched Successfully"));
        }

        // GET: api/Car/GetAllCarsInBranch/{branchId}
        [HttpGet("GetAllCarsInBranch/{branchId}")]
        [ProducesResponseType(typeof(APIResponse<PaginatedResult<CarForGet>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInBranch(Guid branchId, [FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            if (branchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");

            var result = await _mediator.Send(new GetCarsInBranchQuery(branchId, pageNumber, pageSize, search), ct);
            return Ok(APIResponse<PaginatedResult<CarForGet>>.Success(result, "Cars Fetched Successfully"));
        }

        // GET: api/Car/GetAllCarsInCompany/{companyId}
        [HttpGet("GetAllCarsInCompany/{companyId}")]
        [ProducesResponseType(typeof(APIResponse<CarCompanyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCompanyStats(Guid companyId, CancellationToken ct)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var result = await _mediator.Send(new GetCarsInCompanyQuery(companyId), ct);
            return Ok(APIResponse<CarCompanyDto>.Success(result, "stats Cars In Company Returned Successfully"));
        }

        // POST: api/Car/CreateCar
        [HttpPost("CreateCar")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CarForCreateDto dto, CancellationToken ct)
        {
            dto.CreatedBy = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            var result = await _mediator.Send(new CreateCarCommand(dto), ct);
            return Ok(APIResponse<bool>.Success(result, "Car Created Successfully"));
        }

        // PUT: api/Car/UpdateCar/{id}
        [HttpPut("UpdateCar/{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCarDto dto, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid car ID.");

            if (dto == null || id != dto.Id)
                throw new ArgumentException("The ID in the URL does not match the ID in the request body.");
            dto.UpdatedBy = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            var result = await _mediator.Send(new UpdateCarCommand(dto), ct);
            return Ok(APIResponse<bool>.Success(result, "Car Updated Successfully"));
        }

        // DELETE: api/Car/DeleteCar/{id}
        [HttpDelete("DeleteCar/{id}")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid car ID.");

            var result = await _mediator.Send(new DeleteCarCommand(id), ct);
            return Ok(APIResponse<bool>.Success(result, "Car Deleted Successfully"));
        }

        [HttpPost("ImportCarFromExcel")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImportExcel([FromForm] ImportCarExcelRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new ImportCarsFromExcelCommand(
                request.ExcelFile,
                _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System",
                request.BranchId
            ), ct);

            return Ok(APIResponse<bool>.Success(result, "Cars imported successfully."));
        }


        // POST: api/Car/CarSwitchActive/{id}
        [HttpPost("CarSwitchActive/{id}")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CarSwitchActive(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid Car ID.");

            var result = await _mediator.Send(new CarSwitchActiveCommand(id), ct);
            return Ok(APIResponse<bool>.Success(result, "Car status updated successfully."));
        }
    }
}
