using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.Equipment;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FleetLinker.Application.Queries.Equipment.Handlers
{
    public class GetAllEquipmentsQueryHandler : IRequestHandler<GetAllEquipmentsQuery, APIResponse<IEnumerable<EquipmentDto>>>
    {
        private readonly IEquipmentRepository _repository;
        private readonly IAppLocalizer _localizer;

        public GetAllEquipmentsQueryHandler(IEquipmentRepository repository, IAppLocalizer localizer)
        {
            _repository = repository;
            _localizer = localizer;
        }

        public async Task<APIResponse<IEnumerable<EquipmentDto>>> Handle(GetAllEquipmentsQuery request, CancellationToken cancellationToken)
        {
            var equipments = await _repository.GetListAsync();
            var isArabic = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("ar");
            
            var dtos = equipments.Select(e => new EquipmentDto
            {
                Id = e.Id,
                Brand = isArabic 
                    ? (!string.IsNullOrWhiteSpace(e.BrandAr) ? e.BrandAr : e.BrandEn) 
                    : (!string.IsNullOrWhiteSpace(e.BrandEn) ? e.BrandEn : e.BrandAr),
                YearOfManufacture = e.YearOfManufacture,
                ChassisNumber = e.ChassisNumber,
                Model = isArabic 
                    ? (!string.IsNullOrWhiteSpace(e.ModelAr) ? e.ModelAr : e.ModelEn) 
                    : (!string.IsNullOrWhiteSpace(e.ModelEn) ? e.ModelEn : e.ModelAr),
                AssetNumber = e.AssetNumber,
                OwnerId = e.OwnerId,
                OwnerName = e.Owner?.FullName ?? string.Empty
            });

            return APIResponse<IEnumerable<EquipmentDto>>.Success(dtos, _localizer[LocalizationMessages.EquipmentsRetrievedSuccessfully]);
        }
    }

    public class GetEquipmentByIdQueryHandler : IRequestHandler<GetEquipmentByIdQuery, APIResponse<EquipmentDto>>
    {
        private readonly IEquipmentRepository _repository;
        private readonly IAppLocalizer _localizer;

        public GetEquipmentByIdQueryHandler(IEquipmentRepository repository, IAppLocalizer localizer)
        {
            _repository = repository;
            _localizer = localizer;
        }

        public async Task<APIResponse<EquipmentDto>> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
        {
            var equipment = await _repository.GetByGuidAsync(request.Id);
            if (equipment == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.EquipmentNotFound]);

            var isArabic = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("ar");

            var dto = new EquipmentDto
            {
                Id = equipment.Id,
                Brand = isArabic 
                    ? (!string.IsNullOrWhiteSpace(equipment.BrandAr) ? equipment.BrandAr : equipment.BrandEn) 
                    : (!string.IsNullOrWhiteSpace(equipment.BrandEn) ? equipment.BrandEn : equipment.BrandAr),
                YearOfManufacture = equipment.YearOfManufacture,
                ChassisNumber = equipment.ChassisNumber,
                Model = isArabic 
                    ? (!string.IsNullOrWhiteSpace(equipment.ModelAr) ? equipment.ModelAr : equipment.ModelEn) 
                    : (!string.IsNullOrWhiteSpace(equipment.ModelEn) ? equipment.ModelEn : equipment.ModelAr),
                AssetNumber = equipment.AssetNumber,
                OwnerId = equipment.OwnerId,
                OwnerName = equipment.Owner?.FullName ?? string.Empty
            };

            return APIResponse<EquipmentDto>.Success(dto, _localizer[LocalizationMessages.EquipmentRetrievedSuccessfully]);
        }
    }
}
