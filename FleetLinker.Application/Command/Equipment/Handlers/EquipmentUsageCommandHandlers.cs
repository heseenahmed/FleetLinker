using FleetLinker.Application.Common;
using FleetLinker.Application.Common.Interfaces;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FleetLinker.Application.Command.Equipment.Handlers
{
    public class UpdateEquipmentUsageCommandHandler : IRequestHandler<UpdateEquipmentUsageCommand, APIResponse<object?>>
    {
        private readonly IEquipmentRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppLocalizer _localizer;

        public UpdateEquipmentUsageCommandHandler(
            IEquipmentRepository repository,
            UserManager<ApplicationUser> userManager,
            IAppLocalizer localizer)
        {
            _repository = repository;
            _userManager = userManager;
            _localizer = localizer;
        }

        public async Task<APIResponse<object?>> Handle(UpdateEquipmentUsageCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.MechanicalId);
            if (user == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.UserNotFound]);

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("mechanical"))
            {
                throw new UnauthorizedAccessException(_localizer[LocalizationMessages.Unauthorized]);
            }

            var equipment = await _repository.GetByGuidAsync(request.Dto.EquipmentId);
            if (equipment == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.EquipmentNotFound]);

            equipment.UsageHours = request.Dto.UsageHours;
            equipment.FuelLiters = request.Dto.FuelLiters;
            equipment.Kilometers = request.Dto.Kilometers;
            equipment.MechanicalId = request.MechanicalId;
            equipment.UpdatedBy = request.MechanicalId;
            equipment.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(equipment);

            return APIResponse<object?>.Success(null, _localizer[LocalizationMessages.UsageUpdatedSuccessfully]);
        }
    }
}
