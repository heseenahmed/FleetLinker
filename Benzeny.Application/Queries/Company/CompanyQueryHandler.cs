using AutoMapper;
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity.Dto.Company;
using BenzenyMain.Domain.IRepository;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SixLabors.Fonts;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;

namespace BenzenyMain.Application.Queries.Company
{
    public class CompanyQueryHandler :
        IRequestHandler<GetCompanyById, GetCompanyDetailsDto>,
        IRequestHandler<GetCompanyList, APIResponse<PaginatedResult<CompanyDto>>>,
        IRequestHandler<GetAllUserInCompanyQuery, APIResponse<PaginatedResult<GetUserDto>>>,
        IRequestHandler<GetUserByIdInCompanyQuery, APIResponse<GetUserDto>> ,
        IRequestHandler<ExportCompaniesCsvQuery, ExportedFileDto> ,
        IRequestHandler<ExportCompaniesPdfQuery, ExportedFileDto>
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        //static ExportCompaniesPdfQueryHandler()
        //{
        //    QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        //}


        public CompanyQueryHandler(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<GetCompanyDetailsDto> Handle(GetCompanyById request, CancellationToken cancellationToken)
        {
            if (request.CompanyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var result = await _companyRepository.GetCompanyWithUserByIdAsync(request.CompanyId);
            if (result?.Company == null)
                throw new KeyNotFoundException("Company not found.");

            var dto = _mapper.Map<GetCompanyDetailsDto>(result.Company);

            if (result.User != null)
            {
                dto.UserId = result.User.Id.ToString();
                dto.UserName = result.User.UserName;
                dto.Fullname = result.User.FullName;
                dto.Ssn = result.User.SSN;
                dto.Phonenumber = result.User.PhoneNumber;
                dto.Email = result.User.Email;
            }

            dto.FilePaths = result.Company.FilePaths;
            return dto;
        }

        public async Task<APIResponse<PaginatedResult<CompanyDto>>> Handle(GetCompanyList request, CancellationToken cancellationToken)
        {
            if (request.PageNumber < 1 || request.PageSize < 1)
                throw new ArgumentException("Invalid pagination parameters.");

            Expression<Func<Domain.Entity.Company, bool>>? searchExpression = null;
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm;
                searchExpression = c =>
                    c.Name.Contains(term) ||
                    c.CompanyEmail.Contains(term) ||
                    c.CompanyPhone.Contains(term);
            }

            var companies = await _companyRepository.GetPaginatedListAsync(
                predicate: null,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                searchExpression: searchExpression
            );

            var mapped = _mapper.Map<List<CompanyDto>>(companies.Items) ?? new List<CompanyDto>();
            var activeCount = await _companyRepository.GetCompanyCountByActivation(x => x.IsActive);
            var inactiveCount = await _companyRepository.GetCompanyCountByActivation(x => !x.IsActive);

            var pagedResult = new PaginatedResult<CompanyDto>(
                mapped,
                companies.TotalCount,
                companies.PageNumber,
                companies.PageSize,
                activeCount,
                inactiveCount
            );

            return APIResponse<PaginatedResult<CompanyDto>>.Success(pagedResult, "Companies retrieved successfully.");
        }

        public async Task<APIResponse<PaginatedResult<GetUserDto>>> Handle(GetAllUserInCompanyQuery request, CancellationToken cancellationToken)
        {
            if (request.CompanyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            if (request.PageNumber <= 0 || request.PageSize <= 0)
                throw new ArgumentException("Invalid pagination parameters.");

            var users = await _companyRepository.GetAllUsersInCompanyAsync(
                request.CompanyId,
                request.PageNumber,
                request.PageSize,
                request.SearchTerm
            );

            if (users == null || !users.Any())
                throw new KeyNotFoundException("No users found for this company.");

            var stats = await _companyRepository.GetUserStatsInCompanyAsync(request.CompanyId, request.SearchTerm);
            var userDtos = _mapper.Map<List<GetUserDto>>(users) ?? new List<GetUserDto>();

            // map roles
            foreach (var dto in userDtos)
            {
                var user = users.FirstOrDefault(u => u.Id == dto.UserId);
                if (user?.UserRoles != null)
                {
                    dto.UserRoles = user.UserRoles
                        .Where(r => r.Role != null)
                        .Select(r => r.Role.Name)
                        .ToList()!;
                }
            }

            var result = new PaginatedResult<GetUserDto>(
                userDtos,
                stats.Total,
                request.PageNumber,
                request.PageSize,
                stats.Active,
                stats.Inactive
            );

            return APIResponse<PaginatedResult<GetUserDto>>.Success(result, "Users retrieved successfully.");
        }

        public async Task<APIResponse<GetUserDto>> Handle(GetUserByIdInCompanyQuery request, CancellationToken cancellationToken)
        {
            if (request.CompanyId == Guid.Empty || string.IsNullOrWhiteSpace(request.UserId))
                throw new ArgumentException("CompanyId or UserId is invalid.");

            var user = await _companyRepository.GetUserByIdInCompanyAsync(request.CompanyId, request.UserId)
                       ?? throw new KeyNotFoundException("User not found in the given company.");

            var userDto = _mapper.Map<GetUserDto>(user) ?? new GetUserDto();

            if (user.UserRoles != null)
            {
                userDto.UserRoles = user.UserRoles
                    .Where(r => r.Role != null)
                    .Select(r => r.Role.Name)
                    .ToList()!;
            }

            return APIResponse<GetUserDto>.Success(userDto, "User retrieved successfully.");
        }
        public async Task<ExportedFileDto> Handle(ExportCompaniesCsvQuery request, CancellationToken cancellationToken)
        {
            var rows = await _companyRepository.GetAllForCsvAsync(cancellationToken);
            if (rows is null || rows.Count == 0)
                throw new KeyNotFoundException("No companies found to export.");

            // UTF-8 with BOM لتحسين فتح الملف في Excel خاصة مع العربي
            var utf8Bom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
            var sb = new StringBuilder();

            // Header
            sb.AppendLine(string.Join(",", new[]
            {
                "Id","Name","Description","CompanyEmail","CompanyPhone","IBAN",
                "ProfilePicturePath","FilesCount","OwnerUserId","BranchesCount","UsersCount",
                "CreatedDate","UpdatedDate"
            }));

            foreach (var r in rows)
            {
                sb.AppendLine(string.Join(",", new[]
                {
                    Csv(r.Id),
                    Csv(r.Name),
                    Csv(r.Description),
                    Csv(r.CompanyEmail),
                    Csv(r.CompanyPhone),
                    Csv(r.IBAN),
                    Csv(r.ProfilePicturePath),
                    Csv(r.FilesCount),
                    Csv(r.OwnerUserId),
                    Csv(r.BranchesCount),
                    Csv(r.UsersCount),
                    Csv(r.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)),
                    Csv(r.UpdatedDate?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))
                }));
            }

            var bytes = utf8Bom.GetBytes(sb.ToString());
            var fileName = $"companies_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";


            return new ExportedFileDto(bytes, fileName, "text/csv");
        }

        private static string Csv(object? val)
        {
            if (val is null) return "";
            var s = Convert.ToString(val, CultureInfo.InvariantCulture) ?? "";
            var needsQuotes = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
            if (s.Contains('"')) s = s.Replace("\"", "\"\"");
            return needsQuotes ? $"\"{s}\"" : s;
        }
        public async Task<ExportedFileDto> Handle(ExportCompaniesPdfQuery request, CancellationToken cancellationToken)
        {
            var rows = await _companyRepository.GetAllForPdfAsync(cancellationToken);
            if (rows is null || rows.Count == 0)
                throw new KeyNotFoundException("No companies found to export.");

            var nowUtc = DateTime.UtcNow;
            var fileName = $"companies_{nowUtc:yyyyMMdd_HHmmss}.pdf";

            ////(اختياري)دعم خطوط للعربي:
            // var arabicFont = FontCollection.Default.Add(File.OpenRead("wwwroot/fonts/NotoNaskhArabic-Regular.ttf"));
            //var baseStyle = TextStyle.Default.FontSize(9).FontFamily(arabicFont);
            var baseStyle = TextStyle.Default.FontSize(9);

            byte[] pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.DefaultTextStyle(baseStyle);

                    page.Header().Row(row =>
                    {
                        row.ConstantItem(220).Text("Benzeny — Companies Export").SemiBold().FontSize(12);
                        row.RelativeItem().AlignRight().Text($"Generated (UTC): {nowUtc:yyyy-MM-dd HH:mm:ss}");
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(35);   // Id
                            cols.RelativeColumn(1.7f); // Name
                            cols.RelativeColumn(2.2f); // Description
                            cols.RelativeColumn(1.7f); // Email
                            cols.RelativeColumn(1.4f); // Phone
                            cols.RelativeColumn(1.4f); // IBAN
                            cols.RelativeColumn(1.8f); // ProfilePicturePath
                            cols.ConstantColumn(60);   // FilesCount
                            cols.ConstantColumn(85);   // OwnerUserId
                            cols.ConstantColumn(70);   // BranchesCount
                            cols.ConstantColumn(60);   // UsersCount
                            cols.ConstantColumn(110);  // CreatedDate
                            cols.ConstantColumn(110);  // UpdatedDate
                        });

                        // خلايا الهيدر
                        table.Header(header =>
                        {
                            void H(string t) => header.Cell().Element(HeaderCell).Text(t).SemiBold();
                            H("Id"); H("Name"); H("Description"); H("Email"); H("Phone");
                            H("IBAN"); H("ProfilePicture"); H("Files"); H("OwnerUserId");
                            H("Branches"); H("Users"); H("Created"); H("Updated");
                        });

                        // دوال مساعدة للـ body (تتفادى التباس الأنواع)
                        void CellStr(string? t) => table.Cell().Element(BodyCell).Text(t ?? string.Empty);
                        void CellInt(int v) => table.Cell().Element(BodyCell).Text(v.ToString(System.Globalization.CultureInfo.InvariantCulture));

                        foreach (var r in rows)
                        {
                            CellStr(r.Id.ToString());
                            CellStr(r.Name);
                            CellStr(r.Description);
                            CellStr(r.CompanyEmail);
                            CellStr(r.CompanyPhone);
                            CellStr(r.IBAN);
                            CellStr(r.ProfilePicturePath);
                            CellInt(r.FilesCount);
                            CellStr(r.OwnerUserId);
                            CellInt(r.BranchesCount);
                            CellInt(r.UsersCount);

                            // DateTime (non-nullable)
                            CellStr(r.CreatedDate.ToString("yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture));

                            // DateTime? (nullable)
                            CellStr(r.UpdatedDate.HasValue
                                ? r.UpdatedDate.Value.ToString("yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture)
                                : null);
                        }

                        static QuestPDF.Infrastructure.IContainer HeaderCell(QuestPDF.Infrastructure.IContainer c) =>
                            c.PaddingVertical(4).PaddingHorizontal(3).Background(QuestPDF.Helpers.Colors.Grey.Lighten3)
                             .Border(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2);

                        static QuestPDF.Infrastructure.IContainer BodyCell(QuestPDF.Infrastructure.IContainer c) =>
                            c.PaddingVertical(3).PaddingHorizontal(3).BorderBottom(1)
                             .BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten3);
                    });


                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();

            return new ExportedFileDto(pdfBytes, fileName, "application/pdf");
        }
    }
}
