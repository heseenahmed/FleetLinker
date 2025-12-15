
using BenzenyMain.Domain.Entity.Dto.Tag;

namespace BenzenyMain.Application.Contracts.Export
{
    public interface IDriversTemplateBuilder
    {
        byte[] Build(IReadOnlyList<TagLookupDto> tags);

    }
}
