using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Application.Command.CarSettings;
using BenzenyMain.Domain.Entity.Dto.Car;
using BenzenyMain.Domain.Entity.Dto.Tag;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static BenzenyMain.Application.Queries.CarSettings.CarSettingsQuery;

namespace BenzenyMain.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class SettingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region CarBrand

        // POST: api/Settings/CreateCarBrands
        [HttpPost("CreateCarBrands")]
        [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCarBrand([FromBody] CreateCarBrandCommand command, CancellationToken ct)
        {
            if (command == null) throw new ValidationException("Body is required.");

            var result = await _mediator.Send(command, ct);
            return Ok(APIResponse<bool>.Success(result, "Car brand created successfully."));
        }

        // GET: api/Settings/GetAllCarBrands
        [HttpGet("GetAllCarBrands")]
        [ProducesResponseType(typeof(APIResponse<List<CarBrandDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCarBrands(CancellationToken ct)
        {
            var list = await _mediator.Send(new GetAllCarBrandsQuery(), ct);
            return Ok(APIResponse<List<CarBrandDto>>.Success(list, "Car brands retrieved successfully."));
        }

        // PUT: api/Settings/UpdateCarBrands
        [HttpPut("UpdateCarBrands")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCarBrand([FromBody] UpdateCarBrandCommand command, CancellationToken ct)
        {
            if (command == null) throw new ValidationException("Body is required.");

            await _mediator.Send(command, ct);
            return Ok(APIResponse<object>.Success(null, "Car brands updated successfully."));
        }

        // DELETE: api/Settings/DeleteCarBrands/{id}
        [HttpDelete("DeleteCarBrands/{id}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCarBrand(int id, CancellationToken ct)
        {
            if (id <= 0) throw new ArgumentException("Invalid brand ID.");

            await _mediator.Send(new DeleteCarBrandCommand(id), ct);
            return Ok(APIResponse<object>.Success(null, "Car brands deleted successfully."));
        }

        #endregion

        #region CarModel

        // POST: api/Settings/CreateCarModels
        [HttpPost("CreateCarModels")]
        [ProducesResponseType(typeof(APIResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCarModel([FromBody] CreateCarModelCommand command, CancellationToken ct)
        {
            if (command == null) throw new ValidationException("Body is required.");

            var id = await _mediator.Send(command, ct);
            return Ok(APIResponse<int>.Success(id, "Car model created successfully."));
        }

        // GET: api/Settings/GetAllCarModels
        [HttpGet("GetAllCarModels")]
        [ProducesResponseType(typeof(APIResponse<List<CarModelDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCarModels(CancellationToken ct)
        {
            var list = await _mediator.Send(new GetAllCarModelsQuery(), ct);
            return Ok(APIResponse<List<CarModelDto>>.Success(list, "Car models retrieved successfully."));
        }

        // PUT: api/Settings/UpdateCarModels
        [HttpPut("UpdateCarModels")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCarModel([FromBody] UpdateCarModelCommand command, CancellationToken ct)
        {
            if (command == null) throw new ValidationException("Body is required.");

            await _mediator.Send(command, ct);
            return Ok(APIResponse<object>.Success(null, "Car models updated successfully."));
        }

        // DELETE: api/Settings/DeleteCarModel/{id}
        [HttpDelete("DeleteCarModel/{id}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCarModel(int id, CancellationToken ct)
        {
            if (id <= 0) throw new ArgumentException("Invalid model ID.");

            await _mediator.Send(new DeleteCarModelCommand(id), ct);
            return Ok(APIResponse<object>.Success(null, "Car models deleted successfully."));
        }

        #endregion

        #region PlateType

        // POST: api/Settings/CreatePlateTypes
        [HttpPost("CreatePlateTypes")]
        [ProducesResponseType(typeof(APIResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePlateType([FromBody] CreatePlateTypeCommand command, CancellationToken ct)
        {
            if (command == null) throw new ValidationException("Body is required.");

            var id = await _mediator.Send(command, ct);
            return Ok(APIResponse<int>.Success(id, "Plate type created successfully."));
        }

        // GET: api/Settings/GetAllPlateTypes
        [HttpGet("GetAllPlateTypes")]
        [ProducesResponseType(typeof(APIResponse<List<PlateTypeDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPlateTypes(CancellationToken ct)
        {
            var list = await _mediator.Send(new GetAllPlateTypesQuery(), ct);
            return Ok(APIResponse<List<PlateTypeDto>>.Success(list, "Plate types retrieved successfully."));
        }

        // PUT: api/Settings/UpdatePlateTypes
        [HttpPut("UpdatePlateTypes")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePlateType([FromBody] UpdatePlateTypeCommand command, CancellationToken ct)
        {
            if (command == null) throw new ValidationException("Body is required.");

            await _mediator.Send(command, ct);
            return Ok(APIResponse<object>.Success(null, "Plate types updated successfully."));
        }

        // DELETE: api/Settings/DeletePlateTypes/{id}
        [HttpDelete("DeletePlateTypes/{id}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePlateType(int id, CancellationToken ct)
        {
            if (id <= 0) throw new ArgumentException("Invalid plate type ID.");

            await _mediator.Send(new DeletePlateTypeCommand(id), ct);
            return Ok(APIResponse<object>.Success(null, "Plate types deleted successfully."));
        }

        #endregion

        #region CarType

        // POST: api/Settings/CreateCarTypes
        [HttpPost("CreateCarTypes")]
        [ProducesResponseType(typeof(APIResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCarType([FromBody] CreateCarTypeCommand command, CancellationToken ct)
        {
            if (command == null) throw new ValidationException("Body is required.");

            var id = await _mediator.Send(command, ct);
            return Ok(APIResponse<int>.Success(id, "Car type created successfully."));
        }

        // GET: api/Settings/GetAllCarTypes
        [HttpGet("GetAllCarTypes")]
        [ProducesResponseType(typeof(APIResponse<List<CarTypeDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCarTypes(CancellationToken ct)
        {
            var list = await _mediator.Send(new GetAllCarTypesQuery(), ct);
            return Ok(APIResponse<List<CarTypeDto>>.Success(list, "Car types retrieved successfully."));
        }

        // PUT: api/Settings/UpdateCarTypes
        [HttpPut("UpdateCarTypes")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCarType([FromBody] UpdateCarTypeCommand command, CancellationToken ct)
        {
            if (command == null) throw new ValidationException("Body is required.");

            await _mediator.Send(command, ct);
            return Ok(APIResponse<object>.Success(null, "Car types updated successfully."));
        }

        // DELETE: api/Settings/DeleteCarTypes/{id}
        [HttpDelete("DeleteCarTypes/{id}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCarType(int id, CancellationToken ct)
        {
            if (id <= 0) throw new ArgumentException("Invalid car type ID.");

            await _mediator.Send(new DeleteCarTypeCommand(id), ct);
            return Ok(APIResponse<object>.Success(null, "Car types deleted successfully."));
        }

        #endregion

        #region Tag

        // POST: api/Settings/CreateTags
        [HttpPost("CreateTags")]
        [ProducesResponseType(typeof(APIResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagCommand command, CancellationToken ct)
        {
            if (command == null) throw new ValidationException("Body is required.");

            var id = await _mediator.Send(command, ct);
            return Ok(APIResponse<int>.Success(id, "Tag created successfully."));
        }

        // GET: api/Settings/GetAllTags
        [HttpGet("GetAllTags")]
        [ProducesResponseType(typeof(APIResponse<List<TagDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllTags(CancellationToken ct)
        {
            var list = await _mediator.Send(new GetAllTagsQuery(), ct);
            return Ok(APIResponse<List<TagDto>>.Success(list, "Tag retrieved successfully."));
        }

        // PUT: api/Settings/UpdateTags
        [HttpPut("UpdateTags")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTag([FromBody] UpdateTagCommand command, CancellationToken ct)
        {
            if (command == null) throw new ValidationException("Body is required.");

            await _mediator.Send(command, ct);
            return Ok(APIResponse<object>.Success(null, "Tag updated successfully."));
        }

        // DELETE: api/Settings/DeleteTags/{id}
        [HttpDelete("DeleteTags/{id}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTag(int id, CancellationToken ct)
        {
            if (id <= 0) throw new ArgumentException("Invalid tag ID.");

            await _mediator.Send(new DeleteTagCommand(id), ct);
            return Ok(APIResponse<object>.Success(null, "Tag deleted successfully."));
        }

        #endregion
    }
}
