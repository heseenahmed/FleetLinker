
using BenzenyMain.Domain.Entity.Dto.Tag;
using MediatR;

namespace BenzenyMain.Application.Queries.Tag
{
    public sealed record GetAllTagsForTemplateQuery() : IRequest<IReadOnlyList<TagLookupDto>>;
}
