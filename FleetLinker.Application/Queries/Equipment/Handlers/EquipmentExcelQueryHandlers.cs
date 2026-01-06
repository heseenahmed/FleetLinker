using ClosedXML.Excel;
using FleetLinker.Application.Queries.Equipment;
using FleetLinker.Domain.IRepository;
using MediatR;
using System.Data;

namespace FleetLinker.Application.Queries.Equipment.Handlers
{
    public class DownloadEquipmentTemplateQueryHandler : IRequestHandler<DownloadEquipmentTemplateQuery, byte[]>
    {
        private readonly IEquipmentRepository _repository;

        public DownloadEquipmentTemplateQueryHandler(IEquipmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<byte[]> Handle(DownloadEquipmentTemplateQuery request, CancellationToken cancellationToken)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Equipments");
                
                // Headers
                worksheet.Cell(1, 1).Value = "Brand (Arabic)";
                worksheet.Cell(1, 2).Value = "Brand (English)";
                worksheet.Cell(1, 3).Value = "Model (Arabic)";
                worksheet.Cell(1, 4).Value = "Model (English)";
                worksheet.Cell(1, 5).Value = "Year of Manufacture";
                worksheet.Cell(1, 6).Value = "Chassis Number";
                worksheet.Cell(1, 7).Value = "Asset Number";

                // Styling headers
                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Add some sample data or existing data if requested (the user said "with same data of equipment")
                // Let's fetch all equipments
                var equipments = await _repository.GetListAsync();
                int row = 2;
                foreach (var equipment in equipments)
                {
                    worksheet.Cell(row, 1).Value = equipment.BrandAr;
                    worksheet.Cell(row, 2).Value = equipment.BrandEn;
                    worksheet.Cell(row, 3).Value = equipment.ModelAr;
                    worksheet.Cell(row, 4).Value = equipment.ModelEn;
                    worksheet.Cell(row, 5).Value = equipment.YearOfManufacture;
                    worksheet.Cell(row, 6).Value = equipment.ChassisNumber;
                    worksheet.Cell(row, 7).Value = equipment.AssetNumber;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
