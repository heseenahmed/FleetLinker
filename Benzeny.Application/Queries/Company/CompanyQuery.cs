
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity.Dto.Company;
using MediatR;

namespace BenzenyMain.Application.Queries.Company
{
    public record GetCompanyById(Guid CompanyId) : IRequest<GetCompanyDetailsDto>;
    public record GetCompanyList(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null)
    : IRequest<APIResponse<PaginatedResult<CompanyDto>>>;
    public record GetAllUserInCompanyQuery(
    Guid CompanyId,
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null)
    : IRequest<APIResponse<PaginatedResult<GetUserDto>>>;
    public record GetUserByIdInCompanyQuery(Guid CompanyId, string UserId)
        : IRequest<APIResponse<GetUserDto>>;
    public record ExportCompaniesCsvQuery : IRequest<ExportedFileDto>;
    public sealed record ExportCompaniesPdfQuery : IRequest<ExportedFileDto>;
    // DTO عام لملف مُصدّر
    public sealed record ExportedFileDto(byte[] Content, string FileName, string ContentType);
}
