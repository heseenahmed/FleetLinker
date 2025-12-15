
using BenzenyMain.Domain.Entity.Dto.Region;
using MediatR;

namespace BenzenyMain.Application.Command.Region
{
    public record CreateRegionCommand(RegionForCreateDto regionDto) : IRequest<bool>;
    public record UpdateRegionCommand(Guid Id, RegionForUpdateDto regionDto) : IRequest<bool>;
    public record DeleteRegionCommand(Guid Id, string DeletedBy) : IRequest<bool>;

}
