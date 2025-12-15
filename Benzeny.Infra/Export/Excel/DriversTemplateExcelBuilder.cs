
using BenzenyMain.Application.Contracts.Export;
using BenzenyMain.Domain.Entity.Dto.Tag;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BenzenyMain.Infra.Export.Excel
{
    public class DriversTemplateExcelBuilder : IDriversTemplateBuilder
    {
        public byte[] Build(IReadOnlyList<TagLookupDto> tags)
        {
            using var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("Sheet1");

            // headers: added TagName before TagId
            var sheetHeaders = new[] { "License", "LicenseDegree", "FullName", "Phone", "TagName", "TagId" };
            for (int i = 0; i < sheetHeaders.Length; i++)
                ws.Cell(1, i + 1).Value = sheetHeaders[i];

            var headerRange = ws.Range(1, 1, 1, sheetHeaders.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.SheetView.FreezeRows(1);

            // widths
            ws.Column(1).Width = 20; // License
            ws.Column(2).Width = 20; // LicenseDegree
            ws.Column(3).Width = 28; // FullName
            ws.Column(4).Width = 18; // Phone
            ws.Column(5).Width = 24; // TagName (dropdown)
            ws.Column(6).Width = 12; // TagId (calculated)

            // Phone as text
            ws.Column(4).Style.NumberFormat.Format = "@";

            // ---- Lookup on same sheet (hidden) ----
            const int TAG_ID_COL = 7;   // G
            const int TAG_NAME_COL = 8; // H
            ws.Cell(1, TAG_ID_COL).Value = "TagId";
            ws.Cell(1, TAG_NAME_COL).Value = "TagName";
            ws.Range(1, TAG_ID_COL, 1, TAG_NAME_COL).Style.Font.Bold = true;

            for (int i = 0; i < tags.Count; i++)
            {
                ws.Cell(i + 2, TAG_ID_COL).Value = tags[i].Id;
                ws.Cell(i + 2, TAG_NAME_COL).Value = tags[i].Name;
            }

            if (tags.Count > 0)
            {
                var lastRow = tags.Count + 1;
                // Named ranges
                var idsRange = ws.Range(ws.Cell(2, TAG_ID_COL), ws.Cell(lastRow, TAG_ID_COL));
                var namesRange = ws.Range(ws.Cell(2, TAG_NAME_COL), ws.Cell(lastRow, TAG_NAME_COL));
                wb.NamedRanges.Add("TagIds", idsRange);
                wb.NamedRanges.Add("TagNames", namesRange);

                // Data validation on TagName column (E) with names list
                var dvRange = ws.Range("E2:E10000");
                var dv = dvRange.SetDataValidation();
                dv.AllowedValues = XLAllowedValues.List;
                dv.InCellDropdown = true;
                dv.IgnoreBlanks = true;
                dv.ShowErrorMessage = true;
                dv.ErrorStyle = XLErrorStyle.Stop;
                dv.ErrorTitle = "Invalid Tag";
                dv.ErrorMessage = "Please select a Tag from the list.";
                dv.List("=TagNames");

                // TagId column (F) formula to translate name -> id
                // =IF(E2="","",INDEX(TagIds, MATCH(E2, TagNames, 0)))
                for (int r = 2; r <= 10000; r++)
                {
                    ws.Cell(r, 6).FormulaA1 = @"=IF(E" + r + @"="""","""",INDEX(TagIds, MATCH(E" + r + @", TagNames, 0)))";
                }

                // Optional: lock TagId column so users don't type over the formula
                // (requires protecting the sheet if you want to enforce it)
                ws.Column(6).Style.Protection.Locked = true;
            }

            // Hide the lookup columns (G:H)
            ws.Columns(TAG_ID_COL, TAG_NAME_COL).Hide();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }



    }
}
