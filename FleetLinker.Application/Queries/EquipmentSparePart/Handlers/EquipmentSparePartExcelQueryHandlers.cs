using ClosedXML.Excel;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.Queries.EquipmentSparePart;
using FleetLinker.Application.DTOs;
using MediatR;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FleetLinker.Application.Queries.EquipmentSparePart.Handlers
{
    public class DownloadSparePartTemplateQueryHandler : IRequestHandler<DownloadSparePartTemplateQuery, APIResponse<byte[]>>
    {
        private readonly IAppLocalizer _localizer;

        public DownloadSparePartTemplateQueryHandler(IAppLocalizer localizer)
        {
            _localizer = localizer;
        }

        public Task<APIResponse<byte[]>> Handle(DownloadSparePartTemplateQuery request, CancellationToken cancellationToken)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Spare Parts Template");

                // Headers
                worksheet.Cell(1, 1).Value = "Type (Original/Commercial)";
                worksheet.Cell(1, 2).Value = "Part Number";
                worksheet.Cell(1, 3).Value = "Brand (Arabic)";
                worksheet.Cell(1, 4).Value = "Brand (English)";
                worksheet.Cell(1, 5).Value = "Year of Manufacture";
                worksheet.Cell(1, 6).Value = "Asset Number";

                // Formatting
                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return Task.FromResult(APIResponse<byte[]>.Success(content, _localizer[LocalizationMessages.TemplateDownloadedSuccessfully]));
                }
            }
        }
    }
}
