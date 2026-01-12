using FleetLinker.Application.Command.Equipment.EquipmentRequest;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Enums;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FleetLinker.Application.Command.Equipment.EquipmentRequest.Handlers
{
    public class EquipmentRequestCommandHandlers :
        IRequestHandler<CreateEquipmentRequestCommand, APIResponse<bool>>,
        IRequestHandler<RespondToEquipmentRequestCommand, APIResponse<bool>>
    {
        private readonly IEquipmentRequestRepository _repository;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IAppLocalizer _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public EquipmentRequestCommandHandlers(
            IEquipmentRequestRepository repository,
            IEquipmentRepository equipmentRepository,
            IAppLocalizer localizer,
            UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _equipmentRepository = equipmentRepository;
            _localizer = localizer;
            _userManager = userManager;
        }

        public async Task<APIResponse<bool>> Handle(CreateEquipmentRequestCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.RequesterId);
            if (user == null)
                return APIResponse<bool>.Fail(401, message: _localizer[LocalizationMessages.Unauthorized]);

            var equipment = await _equipmentRepository.GetByGuidAsync(request.Dto.EquipmentId);
            if (equipment == null)
                return APIResponse<bool>.Fail(404, message: _localizer[LocalizationMessages.EquipmentNotFound]);

            var equipmentRequest = new Domain.Entity.EquipmentRequest
            {
                Id = Guid.NewGuid(),
                EquipmentId = request.Dto.EquipmentId,
                RequesterId = request.RequesterId,
                OwnerId = equipment.OwnerId,
                RequestType = request.Dto.RequestType,
                Status = EquipmentRequestStatus.Pending,
                RequestedPrice = request.Dto.RequestedPrice,
                Notes = request.Dto.Notes,
                CreatedBy = request.RequesterId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _repository.AddAsync(equipmentRequest);
            return APIResponse<bool>.Success(true, _localizer[LocalizationMessages.EquipmentRequestCreatedSuccessfully]);
        }

        public async Task<APIResponse<bool>> Handle(RespondToEquipmentRequestCommand request, CancellationToken cancellationToken)
        {
            var equipmentRequest = await _repository.GetAsync(
                predicate: x => x.Id == request.Dto.RequestId,
                include: q => q.Include(r => r.Equipment)
            );

            if (equipmentRequest == null)
                return APIResponse<bool>.Fail(404, message: _localizer[LocalizationMessages.NotFound]);

            if (equipmentRequest.OwnerId != request.OwnerId)
                return APIResponse<bool>.Fail(403, message: _localizer[LocalizationMessages.Unauthorized]);

            equipmentRequest.FinalPrice = request.Dto.FinalPrice;
            equipmentRequest.Notes = request.Dto.Notes;
            equipmentRequest.Status = request.Dto.Status;
            equipmentRequest.UpdatedBy = request.OwnerId;
            equipmentRequest.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(equipmentRequest);
            return APIResponse<bool>.Success(true, _localizer[LocalizationMessages.EquipmentRequestRespondedSuccessfully]);
        }
    }
}
