using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Entity.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace FleetLinker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/v1/[action]")]
    [Authorize]
    public class ApiController : ControllerBase
    {
        #region Fields
        public readonly ISender _mediator;
        public readonly UserManager<ApplicationUser> _userManager;
        public string loggedInUserId => _userManager.GetUserId(User) ?? "User";
        #endregion
        #region Constructor
        public ApiController(ISender mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }
        #endregion
        #region Helper Methods
        protected IActionResult? ValidateGuidIdIfEmpty(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new APIResponse<string>
                {
                    Result = "Faild",
                    Msg = "Invalid ID."
                });
            return null;
        }
        #endregion
    }
}