using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.Equipment;
using FleetLinker.Domain.Enums;
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
            IEnumerable<FleetLinker.Domain.Entity.Equipment> equipments;

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                equipments = await _repository.GetListAsync(
                    predicate: e =>
                        ((e.BrandAr != null && e.BrandAr.ToLower().Contains(search)) ||
                        (e.BrandEn != null && e.BrandEn.ToLower().Contains(search)) ||
                        (e.ChassisNumber != null && e.ChassisNumber.ToLower().Contains(search)) ||
                        (e.ModelAr != null && e.ModelAr.ToLower().Contains(search)) ||
                        (e.ModelEn != null && e.ModelEn.ToLower().Contains(search))) &&
                        (e.ForSale || e.ForRent),
                    include: q => q.Include(e => e.Owner).Include(e => e.Mechanical)!
                );
            }
            else
            {
                equipments = await _repository.GetListAsync(
                    predicate: e => e.ForSale || e.ForRent,
                    include: q => q.Include(e => e.Owner).Include(e => e.Mechanical)!
                );
            }

            var isArabic = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("ar");
            
            var dtos = equipments.Select(e => new EquipmentDto
            {
                Id = e.Id,
                Brand = isArabic 
                    ? (!string.IsNullOrWhiteSpace(e.BrandAr) ? e.BrandAr : e.BrandEn) 
                    : (!string.IsNullOrWhiteSpace(e.BrandEn) ? e.BrandEn : e.BrandAr),
                BrandAr = e.BrandAr,
                BrandEn = e.BrandEn ?? string.Empty,
                YearOfManufacture = e.YearOfManufacture,
                ChassisNumber = e.ChassisNumber,
                Model = isArabic 
                    ? (!string.IsNullOrWhiteSpace(e.ModelAr) ? e.ModelAr : e.ModelEn) 
                    : (!string.IsNullOrWhiteSpace(e.ModelEn) ? e.ModelEn : e.ModelAr),
                ModelAr = e.ModelAr,
                ModelEn = e.ModelEn ?? string.Empty,
                AssetNumber = e.AssetNumber,
                OwnerId = e.OwnerId,
                OwnerName = e.Owner?.FullName ?? "N/A",
                ForSale = e.ForSale,
                ForRent = e.ForRent,
                SalePrice = e.SalePrice,
                RentPrice = e.RentPrice,
                ImagePath = e.ImagePath,
                Description = e.Description,
                UsageHours = e.UsageHours,
                FuelLiters = e.FuelLiters,
                MechanicalId = e.MechanicalId,
                MechanicalName = e.Mechanical?.FullName ?? "N/A",
                Kilometers = e.Kilometers,
                EquipmentType = e.EquipmentType == EquipmentType.Truck 
                    ? _localizer[LocalizationMessages.Truck] 
                    : _localizer[LocalizationMessages.Equipment]
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
            var equipment = await _repository.GetAsync(
                predicate: e => e.Id == request.Id,
                include: q => q.Include(e => e.Owner).Include(e => e.Mechanical)!
            );
            if (equipment == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.EquipmentNotFound]);

            var isArabic = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("ar");

            var dto = new EquipmentDto
            {
                Id = equipment.Id,
                Brand = isArabic 
                    ? (!string.IsNullOrWhiteSpace(equipment.BrandAr) ? equipment.BrandAr : equipment.BrandEn) 
                    : (!string.IsNullOrWhiteSpace(equipment.BrandEn) ? equipment.BrandEn : equipment.BrandAr),
                BrandAr = equipment.BrandAr,
                BrandEn = equipment.BrandEn ?? string.Empty,
                YearOfManufacture = equipment.YearOfManufacture,
                ChassisNumber = equipment.ChassisNumber,
                Model = isArabic 
                    ? (!string.IsNullOrWhiteSpace(equipment.ModelAr) ? equipment.ModelAr : equipment.ModelEn) 
                    : (!string.IsNullOrWhiteSpace(equipment.ModelEn) ? equipment.ModelEn : equipment.ModelAr),
                ModelAr = equipment.ModelAr,
                ModelEn = equipment.ModelEn ?? string.Empty,
                AssetNumber = equipment.AssetNumber,
                OwnerId = equipment.OwnerId,
                OwnerName = equipment.Owner?.FullName ?? "N/A",
                ForSale = equipment.ForSale,
                ForRent = equipment.ForRent,
                SalePrice = equipment.SalePrice,
                RentPrice = equipment.RentPrice,
                ImagePath = equipment.ImagePath,
                Description = equipment.Description,
                UsageHours = equipment.UsageHours,
                FuelLiters = equipment.FuelLiters,
                MechanicalId = equipment.MechanicalId,
                MechanicalName = equipment.Mechanical?.FullName ?? "N/A",
                Kilometers = equipment.Kilometers,
                EquipmentType = equipment.EquipmentType == EquipmentType.Truck 
                    ? _localizer[LocalizationMessages.Truck] 
                    : _localizer[LocalizationMessages.Equipment]
            };

            return APIResponse<EquipmentDto>.Success(dto, _localizer[LocalizationMessages.EquipmentRetrievedSuccessfully]);
        }
    }
    public class GetEquipmentsByOwnerQueryHandler : IRequestHandler<GetEquipmentsByOwnerQuery, APIResponse<IEnumerable<EquipmentDto>>>
    {
        private readonly IEquipmentRepository _repository;
        private readonly IAppLocalizer _localizer;

        public GetEquipmentsByOwnerQueryHandler(IEquipmentRepository repository, IAppLocalizer localizer)
        {
            _repository = repository;
            _localizer = localizer;
        }

        public async Task<APIResponse<IEnumerable<EquipmentDto>>> Handle(GetEquipmentsByOwnerQuery request, CancellationToken cancellationToken)
        {
            var equipments = await _repository.GetListAsync(
                predicate: e => e.OwnerId == request.OwnerId,
                include: q => q.Include(e => e.Owner).Include(e => e.Mechanical)!
            );

            var isArabic = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("ar");

            var dtos = equipments.Select(e => new EquipmentDto
            {
                Id = e.Id,
                Brand = isArabic 
                    ? (!string.IsNullOrWhiteSpace(e.BrandAr) ? e.BrandAr : e.BrandEn) 
                    : (!string.IsNullOrWhiteSpace(e.BrandEn) ? e.BrandEn : e.BrandAr),
                BrandAr = e.BrandAr,
                BrandEn = e.BrandEn ?? string.Empty,
                YearOfManufacture = e.YearOfManufacture,
                ChassisNumber = e.ChassisNumber,
                Model = isArabic 
                    ? (!string.IsNullOrWhiteSpace(e.ModelAr) ? e.ModelAr : e.ModelEn) 
                    : (!string.IsNullOrWhiteSpace(e.ModelEn) ? e.ModelEn : e.ModelAr),
                ModelAr = e.ModelAr,
                ModelEn = e.ModelEn ?? string.Empty,
                AssetNumber = e.AssetNumber,
                OwnerId = e.OwnerId,
                OwnerName = e.Owner?.FullName ?? "N/A",
                ForSale = e.ForSale,
                ForRent = e.ForRent,
                SalePrice = e.SalePrice,
                RentPrice = e.RentPrice,
                ImagePath = e.ImagePath,
                Description = e.Description,
                UsageHours = e.UsageHours,
                FuelLiters = e.FuelLiters,
                MechanicalId = e.MechanicalId,
                MechanicalName = e.Mechanical?.FullName ?? "N/A",
                Kilometers = e.Kilometers,
                EquipmentType = e.EquipmentType == EquipmentType.Truck 
                    ? _localizer[LocalizationMessages.Truck] 
                    : _localizer[LocalizationMessages.Equipment]
            });

            return APIResponse<IEnumerable<EquipmentDto>>.Success(dtos, _localizer[LocalizationMessages.EquipmentsRetrievedSuccessfully]);
        }
    }
}
