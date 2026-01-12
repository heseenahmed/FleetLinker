using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.Equipment;
using FleetLinker.Application.Queries.Equipment.EquipmentRequest;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FleetLinker.Application.Queries.Equipment.EquipmentRequest.Handlers
{
    public class EquipmentRequestQueryHandlers :
        IRequestHandler<GetOwnerEquipmentRequestsQuery, APIResponse<List<EquipmentRequestDto>>>,
        IRequestHandler<GetRequesterEquipmentRequestsQuery, APIResponse<List<EquipmentRequestDto>>>
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

        private List<EquipmentRequestDto> MapToDtos(IEnumerable<Domain.Entity.EquipmentRequest> requests)
        {
            var isArabic = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("ar");
            return requests.Select(r => new EquipmentRequestDto
            {
                Id = r.Id,
                EquipmentId = r.EquipmentId,
                EquipmentBrand = isArabic ? r.Equipment?.BrandAr ?? r.Equipment?.BrandEn ?? "N/A" : r.Equipment?.BrandEn ?? "N/A",
                EquipmentModel = isArabic ? r.Equipment?.ModelAr ?? r.Equipment?.ModelEn ?? "N/A" : r.Equipment?.ModelEn ?? "N/A",
                RequesterId = r.RequesterId,
                RequesterName = r.Requester?.FullName ?? "N/A",
                OwnerId = r.OwnerId,
                RequestType = r.RequestType,
                RequestTypeName = r.RequestType.ToString(),
                Status = r.Status,
                StatusName = r.Status.ToString(),
                RequestedPrice = r.RequestedPrice,
                FinalPrice = r.FinalPrice,
                Notes = r.Notes,
                CreatedDate = r.CreatedDate
            }).ToList();
        }
    }
}
