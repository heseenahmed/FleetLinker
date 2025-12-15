
using Benzeny.Domain.IRepository;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Tag;

namespace BenzenyMain.Domain.IRepository
{
    public interface ITagRepository : IBaseRepository<Tag>
    {
        Task<IReadOnlyList<TagLookupDto>> GetAllAsync(CancellationToken ct);

    }
}
