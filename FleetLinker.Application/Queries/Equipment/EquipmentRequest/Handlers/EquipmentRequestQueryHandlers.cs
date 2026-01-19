using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.Equipment;
using FleetLinker.Application.Queries.Equipment.EquipmentRequest;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FleetLinker.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FleetLinker.Application.Queries.Equipment.EquipmentRequest.Handlers
{
    public class EquipmentRequestQueryHandlers :
        IRequestHandler<GetOwnerEquipmentRequestsQuery, APIResponse<List<EquipmentRequestDto>>>,
        IRequestHandler<GetRequesterEquipmentRequestsQuery, APIResponse<List<EquipmentRequestDto>>>,
        IRequestHandler<GetMechanicalMaintenanceRequestsQuery, APIResponse<List<EquipmentRequestDto>>>,
        IRequestHandler<GetOwnerMaintenanceRequestsQuery, APIResponse<List<EquipmentRequestDto>>>
    {
        private readonly IEquipmentRequestRepository _repository;
        private readonly IAppLocalizer _localizer;

        public EquipmentRequestQueryHandlers(IEquipmentRequestRepository repository, IAppLocalizer localizer)
        {
            _repository = repository;
            _localizer = localizer;
        }

        public async Task<APIResponse<List<EquipmentRequestDto>>> Handle(GetOwnerEquipmentRequestsQuery request, CancellationToken cancellationToken)
        {
            var requests = await _repository.GetListAsync(
                predicate: x => x.OwnerId == request.OwnerId && x.IsActive,
                include: q => q.Include(r => r.Equipment).Include(r => r.Requester)
            );

            var dtos = MapToDtos(requests);
            return APIResponse<List<EquipmentRequestDto>>.Success(dtos, _localizer[LocalizationMessages.EquipmentRequestsRetrievedSuccessfully]);
        }

        public async Task<APIResponse<List<EquipmentRequestDto>>> Handle(GetRequesterEquipmentRequestsQuery request, CancellationToken cancellationToken)
        {
            var requests = await _repository.GetListAsync(
                predicate: x => x.RequesterId == request.RequesterId && x.IsActive,
                include: q => q.Include(r => r.Equipment).Include(r => r.Requester)
            );

            var dtos = MapToDtos(requests);
            return APIResponse<List<EquipmentRequestDto>>.Success(dtos, _localizer[LocalizationMessages.EquipmentRequestsRetrievedSuccessfully]);
        }

        public async Task<APIResponse<List<EquipmentRequestDto>>> Handle(GetMechanicalMaintenanceRequestsQuery request, CancellationToken cancellationToken)
        {
            var requests = await _repository.GetListAsync(
                predicate: x => x.RequesterId == request.MechanicalId && x.RequestType == EquipmentRequestType.Maintenance && x.IsActive,
                include: q => q.Include(r => r.Equipment).Include(r => r.Requester)
            );

            var dtos = MapToDtos(requests);
            return APIResponse<List<EquipmentRequestDto>>.Success(dtos, _localizer[LocalizationMessages.EquipmentRequestsRetrievedSuccessfully]);
        }

        public async Task<APIResponse<List<EquipmentRequestDto>>> Handle(GetOwnerMaintenanceRequestsQuery request, CancellationToken cancellationToken)
        {
            var requests = await _repository.GetListAsync(
                predicate: x => x.OwnerId == request.OwnerId && x.RequestType == EquipmentRequestType.Maintenance && x.IsActive,
                include: q => q.Include(r => r.Equipment).Include(r => r.Requester)
            );

            var dtos = MapToDtos(requests);
            return APIResponse<List<EquipmentRequestDto>>.Success(dtos, _localizer[LocalizationMessages.EquipmentRequestsRetrievedSuccessfully]);
        }

        private List<EquipmentRequestDto> MapToDtos(IEnumerable<Domain.Entity.EquipmentRequest> requests)
        {
            var isArabic = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("ar");
            return requests.Select(r => new EquipmentRequestDto
            {
                Id = r.Id,
                EquipmentId = r.EquipmentId,
                EquipmentBrand = isArabic 
                    ? (!string.IsNullOrWhiteSpace(r.Equipment?.BrandAr) ? r.Equipment.BrandAr : !string.IsNullOrWhiteSpace(r.Equipment?.BrandEn) ? r.Equipment.BrandEn : "N/A") 
                    : (!string.IsNullOrWhiteSpace(r.Equipment?.BrandEn) ? r.Equipment.BrandEn : "N/A"),
                EquipmentModel = isArabic 
                    ? (!string.IsNullOrWhiteSpace(r.Equipment?.ModelAr) ? r.Equipment.ModelAr : !string.IsNullOrWhiteSpace(r.Equipment?.ModelEn) ? r.Equipment.ModelEn : "N/A") 
                    : (!string.IsNullOrWhiteSpace(r.Equipment?.ModelEn) ? r.Equipment.ModelEn : "N/A"),
                RequesterId = r.RequesterId,
                RequesterName = r.Requester?.FullName ?? "N/A",
                OwnerId = r.OwnerId,
                RequestType = GetLocalizedRequestType(r.RequestType),
                Status = GetLocalizedStatus(r.Status),
                RequestedPrice = r.RequestedPrice,
                FinalPrice = r.FinalPrice,
                MaintenanceDescription = r.MaintenanceDescription,
                MaintenanceResponse = r.MaintenanceResponse,
                Notes = r.Notes,
                CreatedDate = r.CreatedDate
            }).ToList();
        }

        private string GetLocalizedRequestType(EquipmentRequestType type)
        {
            return type switch
            {
                EquipmentRequestType.Buy => _localizer[LocalizationMessages.RequestTypeBuy],
                EquipmentRequestType.Rent => _localizer[LocalizationMessages.RequestTypeRent],
                EquipmentRequestType.Maintenance => _localizer[LocalizationMessages.RequestTypeMaintenance],
                _ => type.ToString()
            };
        }

        private string GetLocalizedStatus(EquipmentRequestStatus status)
        {
            return status switch
            {
                EquipmentRequestStatus.Pending => _localizer[LocalizationMessages.StatusPending],
                EquipmentRequestStatus.Responded => _localizer[LocalizationMessages.StatusResponded],
                EquipmentRequestStatus.Accepted => _localizer[LocalizationMessages.StatusAccepted],
                EquipmentRequestStatus.Rejected => _localizer[LocalizationMessages.StatusRejected],
                _ => status.ToString()
            };
        }
    }
}
