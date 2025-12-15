
using BenzenyMain.Domain.Entity.Dto.Tag;
using BenzenyMain.Domain.IRepository;
using MediatR;

namespace BenzenyMain.Application.Queries.Tag
{
    public class TagQueryHandler : IRequestHandler<GetAllTagsForTemplateQuery, IReadOnlyList<TagLookupDto>>
    {
        private readonly ITagRepository _repo;
        public TagQueryHandler(ITagRepository repo) => _repo = repo;

        public Task<IReadOnlyList<TagLookupDto>> Handle(GetAllTagsForTemplateQuery request, CancellationToken ct)
        => _repo.GetAllAsync(ct);
    }
}
