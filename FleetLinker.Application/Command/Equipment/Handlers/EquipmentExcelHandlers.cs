using ClosedXML.Excel;
using FleetLinker.Application.Command.Equipment;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.IRepository;
using MediatR;

namespace FleetLinker.Application.Command.Equipment.Handlers
{
    public class UploadEquipmentExcelCommandHandler : IRequestHandler<UploadEquipmentExcelCommand, APIResponse<object?>>
    {
        private readonly IEquipmentRepository _repository;
        private readonly IAppLocalizer _localizer;

        public UploadEquipmentExcelCommandHandler(IEquipmentRepository repository, IAppLocalizer localizer)
        {
            _repository = repository;
            _localizer = localizer;
        }

        public async Task<APIResponse<object?>> Handle(UploadEquipmentExcelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using (var workbook = new XLWorkbook(request.FileStream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

                    var equipmentsToAdd = new List<Domain.Entity.Equipment>();

                    foreach (var row in rows)
                    {
                        var brandAr = row.Cell(1).GetValue<string>();
                        var brandEn = row.Cell(2).GetValue<string>();
                        var modelAr = row.Cell(3).GetValue<string>();
                        var modelEn = row.Cell(4).GetValue<string>();
                        var yearStr = row.Cell(5).GetValue<string>();
                        var chassisNumber = row.Cell(6).GetValue<string>();
                        var assetNumber = row.Cell(7).GetValue<string>();

                        if (string.IsNullOrWhiteSpace(brandEn) || string.IsNullOrWhiteSpace(modelEn) || 
                            string.IsNullOrWhiteSpace(chassisNumber) || !int.TryParse(yearStr, out int year))
                        {
                            continue; // Basic validation
                        }

                        equipmentsToAdd.Add(new Domain.Entity.Equipment
                        {
                            BrandAr = brandAr,
                            BrandEn = brandEn,
                            ModelAr = modelAr,
                            ModelEn = modelEn,
                            YearOfManufacture = year,
                            ChassisNumber = chassisNumber,
                            AssetNumber = assetNumber,
                            OwnerId = request.CreatedBy,
                            CreatedBy = request.CreatedBy,
                            CreatedDate = DateTime.UtcNow,
                            IsActive = true
                        });
                    }

                    if (equipmentsToAdd.Any())
                    {
                        await _repository.AddRangeAsync(equipmentsToAdd);
                    }

                    return APIResponse<object?>.Success(null, _localizer[LocalizationMessages.EquipmentsAddedSuccessfully]);
                }
            }
            catch (Exception ex)
            {
                return APIResponse<object?>.Exception(ex, _localizer[LocalizationMessages.Error]);
            }
        }
    }
}
