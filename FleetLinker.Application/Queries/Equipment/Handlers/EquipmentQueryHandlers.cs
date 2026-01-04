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
            var equipments = await _repository._dbSet
                .Include(e => e.Owner)
                .Where(e => e.IsActive)
                .ToListAsync(cancellationToken);

            var data = equipments.Select(e => new EquipmentDto
            {
                Id = e.Id,
                Brand = e.Brand,
                YearOfManufacture = e.YearOfManufacture,
                ChassisNumber = e.ChassisNumber,
                Model = e.Model,
                AssetNumber = e.AssetNumber,
                OwnerId = e.OwnerId,
                OwnerName = e.Owner.FullName
            });

            return APIResponse<IEnumerable<EquipmentDto>>.Success(data, _localizer[LocalizationMessages.EquipmentsRetrievedSuccessfully]);
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
            var e = await _repository._dbSet
                .Include(e => e.Owner)
                .FirstOrDefaultAsync(e => e.Id == request.Id && e.IsActive, cancellationToken);

            if (e == null) throw new KeyNotFoundException("Equipment not found.");

            var data = new EquipmentDto
            {
                Id = e.Id,
                Brand = e.Brand,
                YearOfManufacture = e.YearOfManufacture,
                ChassisNumber = e.ChassisNumber,
                Model = e.Model,
                AssetNumber = e.AssetNumber,
                OwnerId = e.OwnerId,
                OwnerName = e.Owner.FullName
            };

            return APIResponse<EquipmentDto>.Success(data, _localizer[LocalizationMessages.EquipmentRetrievedSuccessfully]);
        }
    }
}
