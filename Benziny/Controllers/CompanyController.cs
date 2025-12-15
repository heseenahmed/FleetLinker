using Benzeny.Application.Common;
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Application.Command.Company;
using BenzenyMain.Application.Queries.Company;
using BenzenyMain.Domain.Entity.Dto.Company;
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
    public class CompanyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompanyController(IMediator mediator , IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        // POST: api/Company/CreateCompany
        [HttpPost("CreateCompany")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromForm] CompanyForCreateDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                throw new ValidationException(string.Join(" | ", errors));
            }

            var isSuccess = await _mediator.Send(new CreateCompanyCommand(model), ct);
            if (!isSuccess)
                throw new ApplicationException("Failed to create the company.");

            return Ok(APIResponse<object>.Success(null, "Company created successfully."));
        }

        // GET: api/Company/GetCompanyById/{id}
        [HttpGet("GetCompanyById/{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(APIResponse<GetCompanyDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var company = await _mediator.Send(new GetCompanyById(id), ct)
                          ?? throw new KeyNotFoundException("Company not found.");

            return Ok(APIResponse<GetCompanyDetailsDto>.Success(company, "Company retrieved successfully."));
        }

        // PUT: api/Company/UpdateCompany/{id}
        [HttpPut("UpdateCompany/{id:guid}")]
        [AllowAnonymous] // remove later based on your auth rules
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromForm] CompanyForUpdateDto dto, CancellationToken ct)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            if (id != dto.Id)
                throw new ArgumentException("The ID in the URL does not match the ID in the request body.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                throw new ValidationException(string.Join(" | ", errors));
            }

            var updatedBy = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
            var result = await _mediator.Send(new UpdateCompanyCommand(id, dto, updatedBy), ct); // returns APIResponse with ApiStatusCode
            return StatusCode(result.ApiStatusCode, result);
        }

        // GET: api/Company/GetAllCompanies
        [HttpGet("GetAllCompanies")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetCompanyList(pageNumber, pageSize, searchTerm), ct); // returns APIResponse
            return StatusCode(result.ApiStatusCode, result);
        }

        // GET: api/Company/GetAllUserInCompany/{companyId}
        [HttpGet("GetAllUserInCompany/{companyId:guid}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUserInCompany(
            Guid companyId,
            [FromQuery] string? searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var result = await _mediator.Send(new GetAllUserInCompanyQuery(companyId, pageNumber, pageSize, searchTerm), ct); // returns APIResponse
            return StatusCode(result.ApiStatusCode, result);
        }

        // GET: api/Company/GetUserByIdInCompany/{companyId}/{userId}
        [HttpGet("GetUserByIdInCompany/{companyId:guid}/{userId}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByIdInCompany(Guid companyId, string userId, CancellationToken ct)
        {
            if (companyId == Guid.Empty || string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid company or user ID.");

            var result = await _mediator.Send(new GetUserByIdInCompanyQuery(companyId, userId), ct); // returns APIResponse
            return StatusCode(result.ApiStatusCode, result);
        }

        // POST: api/Company/CompanySwitchActive/{companyId}
        [HttpPost("CompanySwitchActive/{companyId:guid}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CompanySwitchActive(Guid companyId, CancellationToken ct)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            // Extract current username or email
            var performedBy = User?.Identity?.Name ?? "System";

            var result = await _mediator.Send(new SwitchActiveCompanyCommand(companyId , performedBy), ct); // returns APIResponse
            return StatusCode(result.ApiStatusCode, result);
        }

        // DELETE: api/Company/DeleteCompany/{companyId}
        [HttpDelete("DeleteCompany/{companyId:guid}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCompany(Guid companyId, CancellationToken ct)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var performedBy = User?.Identity?.Name ?? "System";
            
            var response = await _mediator.Send(new DeleteCompanyCommand(companyId , performedBy), ct); // returns APIResponse
            return StatusCode(response.ApiStatusCode, response);
        }
        [HttpGet("ExportCsv")]
        public async Task<IActionResult> ExportCsv(CancellationToken ct)
        {
            var file = await _mediator.Send(new ExportCompaniesCsvQuery(), ct);
            return File(file.Content, file.ContentType, file.FileName);
        }
        [HttpGet("ExportPdf")]
        public async Task<IActionResult> ExportPdf(CancellationToken ct)
        {
            var file = await _mediator.Send(new ExportCompaniesPdfQuery(), ct);
            return File(file.Content, file.ContentType, file.FileName);
        }
    }
}
