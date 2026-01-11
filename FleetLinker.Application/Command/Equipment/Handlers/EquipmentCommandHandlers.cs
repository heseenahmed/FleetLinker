using FleetLinker.Application.Common.Interfaces;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Enums;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FleetLinker.Application.Command.Equipment.Handlers
{
    public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, APIResponse<object?>>
    {
        private readonly IEquipmentRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppLocalizer _localizer;

        public CreateEquipmentCommandHandler(
            IEquipmentRepository repository,
            UserManager<ApplicationUser> userManager,
            IAppLocalizer localizer)
        {
            _repository = repository;
            _userManager = userManager;
            _localizer = localizer;
        }

        public async Task<APIResponse<object?>> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.CreatedBy);
            if (user == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.UserNotFound]);

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Equipment owner"))
            {
                throw new UnauthorizedAccessException(_localizer[LocalizationMessages.EquipmentUnauthorized]);
            }

            var equipment = new Domain.Entity.Equipment
            {
                BrandAr = request.Dto.BrandAr,
                BrandEn = request.Dto.BrandEn,
                YearOfManufacture = request.Dto.YearOfManufacture!.Value,
                ChassisNumber = request.Dto.ChassisNumber,
                ModelAr = request.Dto.ModelAr,
                ModelEn = request.Dto.ModelEn,
                AssetNumber = request.Dto.AssetNumber,
                OwnerId = request.CreatedBy,
                CreatedBy = request.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _repository.AddAsync(equipment);
            return APIResponse<object?>.Success(null, _localizer[LocalizationMessages.EquipmentCreatedSuccessfully]);
        }
    }

    public class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand, APIResponse<object?>>
    {
        private readonly IEquipmentRepository _repository;
        private readonly IAppLocalizer _localizer;

        public UpdateEquipmentCommandHandler(IEquipmentRepository repository, IAppLocalizer localizer)
        {
            _repository = repository;
            _localizer = localizer;
        }

        public async Task<APIResponse<object?>> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
        {
    var equipment = await _repository.GetByGuidAsync(request.Dto.Id);
    if (equipment == null) throw new KeyNotFoundException(_localizer[LocalizationMessages.EquipmentNotFound]);

    equipment.BrandAr = request.Dto.BrandAr;
    equipment.BrandEn = request.Dto.BrandEn;
    equipment.YearOfManufacture = request.Dto.YearOfManufacture!.Value;
    equipment.ChassisNumber = request.Dto.ChassisNumber;
    equipment.ModelAr = request.Dto.ModelAr;
    equipment.ModelEn = request.Dto.ModelEn;
    equipment.AssetNumber = request.Dto.AssetNumber;
    equipment.UpdatedBy = request.UpdatedBy;
    equipment.UpdatedDate = DateTime.UtcNow;

    await _repository.UpdateAsync(equipment);
    return APIResponse<object?>.Success(null, _localizer[LocalizationMessages.EquipmentUpdatedSuccessfully]);
}
    }

    public class DeleteEquipmentCommandHandler : IRequestHandler<DeleteEquipmentCommand, APIResponse<object?>>
    {
        private readonly IEquipmentRepository _repository;
        private readonly IAppLocalizer _localizer;

        public DeleteEquipmentCommandHandler(IEquipmentRepository repository, IAppLocalizer localizer)
        {
            _repository = repository;
            _localizer = localizer;
        }

        public async Task<APIResponse<object?>> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
        {
            var equipment = await _repository.GetByGuidAsync(request.Id);
            if (equipment == null) throw new KeyNotFoundException(_localizer[LocalizationMessages.EquipmentNotFound]);

            await _repository.RemoveAsync(equipment);
            return APIResponse<object?>.Success(null, _localizer[LocalizationMessages.EquipmentDeletedSuccessfully]);
        }
    }
}
