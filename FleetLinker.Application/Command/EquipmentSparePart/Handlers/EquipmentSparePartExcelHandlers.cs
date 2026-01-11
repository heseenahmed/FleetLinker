using ClosedXML.Excel;
using FleetLinker.Application.Command.EquipmentSparePart;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Enums;
using FleetLinker.Domain.IRepository;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FleetLinker.Application.Command.EquipmentSparePart.Handlers
{
    public class BatchUploadSparePartsCommandHandler : IRequestHandler<BatchUploadSparePartsCommand, APIResponse<bool>>
    {
        private readonly IEquipmentSparePartRepository _sparePartRepository;
        private readonly IAppLocalizer _localizer;

        public BatchUploadSparePartsCommandHandler(IEquipmentSparePartRepository sparePartRepository, IAppLocalizer localizer)
        {
            _sparePartRepository = sparePartRepository;
            _localizer = localizer;
        }

        public async Task<APIResponse<bool>> Handle(BatchUploadSparePartsCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length <= 0)
            {
                return APIResponse<bool>.Fail(400, message: _localizer[LocalizationMessages.InvalidExcelFile]);
            }

            if (!Path.GetExtension(request.File.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return APIResponse<bool>.Fail(400, message: _localizer[LocalizationMessages.InvalidExcelFile]);
            }

            var spareParts = new List<FleetLinker.Domain.Entity.EquipmentSparePart>();

            using (var stream = new MemoryStream())
            {
                await request.File.CopyToAsync(stream, cancellationToken);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed().Skip(1); // Skip header row

                    foreach (var row in rows)
                    {
                        var typeStr = row.Cell(1).GetString();
                        var partNumber = row.Cell(2).GetString();
                        var brandAr = row.Cell(3).GetString();
                        var brandEn = row.Cell(4).GetString();
                        var yearStr = row.Cell(5).GetString();
                        var assetNumber = row.Cell(6).GetString();

                        if (string.IsNullOrWhiteSpace(partNumber)) continue;

                        var type = typeStr.Equals("Original", StringComparison.OrdinalIgnoreCase) || typeStr.Equals("أصلي", StringComparison.OrdinalIgnoreCase)
                            ? PartType.Original
                            : PartType.Commercial;

                        int.TryParse(yearStr, out int year);

                        var sparePart = new FleetLinker.Domain.Entity.EquipmentSparePart
                        {
                            Id = Guid.NewGuid(),
                            Type = type,
                            PartNumber = partNumber,
                            BrandAr = brandAr,
                            BrandEn = brandEn,
                            YearOfManufacture = year,
                            AssetNumber = assetNumber,
                            SupplierId = request.SupplierId,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = request.SupplierId,
                            IsActive = true
                        };

                        spareParts.Add(sparePart);
                    }
                }
            }

            if (spareParts.Any())
            {
                await _sparePartRepository.AddRangeAsync(spareParts);
            }

            return APIResponse<bool>.Success(true, _localizer[LocalizationMessages.SparePartsUploadedSuccessfully]);
        }
    }
}
