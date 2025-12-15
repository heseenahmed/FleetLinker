using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Application.Queries.Log;
using BenzenyMain.Domain.Entity.Dto.Log;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BenzenyMain.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LogController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<PaginatedResult<LogForListDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllLogs(
             [FromQuery] int pageNumber = 1,
             [FromQuery] int pageSize = 20,
             [FromQuery] string? searchTerm = null,
             [FromQuery] DateTime? fromDate = null,
             [FromQuery] DateTime? toDate = null,
             CancellationToken ct = default)
        {
            var query = new GetLogsQuery(pageNumber, pageSize , searchTerm , fromDate, toDate);
            var response = await _mediator.Send(query, ct);
            return StatusCode(response.ApiStatusCode, response);
        }
    }
}
