using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.EquipmentSparePart;
using FleetLinker.Domain.IRepository;
using MediatR;

namespace FleetLinker.Application.Queries.EquipmentSparePart.Handlers
{
    public class EquipmentSparePartQueryHandlers :
        IRequestHandler<GetAllEquipmentSparePartsQuery, APIResponse<IEnumerable<EquipmentSparePartDto>>>,
        IRequestHandler<GetEquipmentSparePartByIdQuery, APIResponse<EquipmentSparePartDto>>
    {
        private readonly IEquipmentSparePartRepository _repository;
        private readonly IAppLocalizer _localizer;

        public EquipmentSparePartQueryHandlers(IEquipmentSparePartRepository repository, IAppLocalizer localizer)
        {
            _repository = repository;
            _localizer = localizer;
        }

        public async Task<APIResponse<IEnumerable<EquipmentSparePartDto>>> Handle(GetAllEquipmentSparePartsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entity.EquipmentSparePart> parts;

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                parts = await _repository.GetListAsync(p =>
                    p.PartNumber.ToLower().Contains(search) ||
                    (p.BrandAr != null && p.BrandAr.ToLower().Contains(search)) ||
                    (p.BrandEn != null && p.BrandEn.ToLower().Contains(search))
                );
            }
            else
            {
                parts = await _repository.GetListAsync();
            }

            var isArabic = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("ar");

            var dtos = parts.Select(p => new EquipmentSparePartDto
            {
                Id = p.Id,
                Type = isArabic 
                    ? (p.Type == FleetLinker.Domain.Enums.PartType.Original ? "أصلي" : "تجاري")
                    : (p.Type == FleetLinker.Domain.Enums.PartType.Original ? "Original" : "Commercial"),
                TypeAr = p.Type == FleetLinker.Domain.Enums.PartType.Original ? "أصلي" : "تجاري",
                TypeEn = p.Type == FleetLinker.Domain.Enums.PartType.Original ? "Original" : "Commercial",
                PartNumber = p.PartNumber,
                Brand = isArabic 
                    ? (!string.IsNullOrWhiteSpace(p.BrandAr) ? p.BrandAr : p.BrandEn ?? string.Empty) 
                    : (!string.IsNullOrWhiteSpace(p.BrandEn) ? p.BrandEn : p.BrandAr ?? string.Empty),
                BrandAr = p.BrandAr,
                BrandEn = p.BrandEn,
                YearOfManufacture = p.YearOfManufacture,
                AssetNumber = p.AssetNumber,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier?.FullName ?? string.Empty
            });

            return APIResponse<IEnumerable<EquipmentSparePartDto>>.Success(dtos, _localizer[LocalizationMessages.SparePartsRetrievedSuccessfully]);
        }

        public async Task<APIResponse<EquipmentSparePartDto>> Handle(GetEquipmentSparePartByIdQuery request, CancellationToken cancellationToken)
        {
            var part = await _repository.GetByGuidAsync(request.Id);
            if (part == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.SparePartNotFound]);

            var isArabic = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("ar");

            var dto = new EquipmentSparePartDto
            {
                Id = part.Id,
                Type = isArabic 
                    ? (part.Type == FleetLinker.Domain.Enums.PartType.Original ? "أصلي" : "تجاري")
                    : (part.Type == FleetLinker.Domain.Enums.PartType.Original ? "Original" : "Commercial"),
                TypeAr = part.Type == FleetLinker.Domain.Enums.PartType.Original ? "أصلي" : "تجاري",
                TypeEn = part.Type == FleetLinker.Domain.Enums.PartType.Original ? "Original" : "Commercial",
                PartNumber = part.PartNumber,
                Brand = isArabic 
                    ? (!string.IsNullOrWhiteSpace(part.BrandAr) ? part.BrandAr : part.BrandEn ?? string.Empty) 
                    : (!string.IsNullOrWhiteSpace(part.BrandEn) ? part.BrandEn : part.BrandAr ?? string.Empty),
                BrandAr = part.BrandAr,
                BrandEn = part.BrandEn,
                YearOfManufacture = part.YearOfManufacture,
                AssetNumber = part.AssetNumber,
                SupplierId = part.SupplierId,
                SupplierName = part.Supplier?.FullName ?? string.Empty
            };

            return APIResponse<EquipmentSparePartDto>.Success(dto, _localizer[LocalizationMessages.SparePartRetrievedSuccessfully]);
        }
    }
}
