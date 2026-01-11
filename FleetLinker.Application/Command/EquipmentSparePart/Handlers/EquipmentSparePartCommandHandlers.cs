using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FleetLinker.Application.Command.EquipmentSparePart.Handlers
{
    public class EquipmentSparePartCommandHandlers :
        IRequestHandler<CreateEquipmentSparePartCommand, APIResponse<object?>>,
        IRequestHandler<UpdateEquipmentSparePartCommand, APIResponse<object?>>,
        IRequestHandler<DeleteEquipmentSparePartCommand, APIResponse<object?>>
    {
        private readonly IEquipmentSparePartRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppLocalizer _localizer;

        public EquipmentSparePartCommandHandlers(
            IEquipmentSparePartRepository repository,
            UserManager<ApplicationUser> userManager,
            IAppLocalizer localizer)
        {
            _repository = repository;
            _userManager = userManager;
            _localizer = localizer;
        }

        public async Task<APIResponse<object?>> Handle(CreateEquipmentSparePartCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.SupplierId);
            if (user == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.UserNotFound]);

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Supplier"))
            {
                throw new UnauthorizedAccessException(_localizer[LocalizationMessages.EquipmentUnauthorized]); // Reusing for now or add new message
            }

            var part = new Domain.Entity.EquipmentSparePart
            {
                Type = request.Dto.Type,
                PartNumber = request.Dto.PartNumber,
                BrandAr = request.Dto.BrandAr,
                BrandEn = request.Dto.BrandEn,
                YearOfManufacture = request.Dto.YearOfManufacture,
                AssetNumber = request.Dto.AssetNumber,
                SupplierId = request.SupplierId,
                CreatedBy = request.SupplierId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _repository.AddAsync(part);
            return APIResponse<object?>.Success(null, _localizer[LocalizationMessages.SparePartCreatedSuccessfully]);
        }

        public async Task<APIResponse<object?>> Handle(UpdateEquipmentSparePartCommand request, CancellationToken cancellationToken)
        {
            var part = await _repository.GetByGuidAsync(request.Dto.Id);
            if (part == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.SparePartNotFound]);

            if (part.SupplierId != request.SupplierId)
                throw new UnauthorizedAccessException(_localizer[LocalizationMessages.EquipmentUnauthorized]);

            part.Type = request.Dto.Type;
            part.PartNumber = request.Dto.PartNumber;
            part.BrandAr = request.Dto.BrandAr;
            part.BrandEn = request.Dto.BrandEn;
            part.YearOfManufacture = request.Dto.YearOfManufacture;
            part.AssetNumber = request.Dto.AssetNumber;
            part.UpdatedBy = request.SupplierId;
            part.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(part);
            return APIResponse<object?>.Success(null, _localizer[LocalizationMessages.SparePartUpdatedSuccessfully]);
        }

        public async Task<APIResponse<object?>> Handle(DeleteEquipmentSparePartCommand request, CancellationToken cancellationToken)
        {
            var part = await _repository.GetByGuidAsync(request.Id);
            if (part == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.SparePartNotFound]);

            if (part.SupplierId != request.SupplierId)
                throw new UnauthorizedAccessException(_localizer[LocalizationMessages.EquipmentUnauthorized]);

            await _repository.RemoveAsync(part);
            return APIResponse<object?>.Success(null, _localizer[LocalizationMessages.SparePartDeletedSuccessfully]);
        }
    }
}
