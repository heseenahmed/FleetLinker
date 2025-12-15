using FleetLinker.Domain.Entity.Dto.Identity;
using FleetLinker.Domain.Entity.Dto.User;
using MediatR;
namespace FleetLinker.Application.Queries.User
{
    public sealed record GetUserInfoAsyncCommand(string Id) : IRequest<UserInfoAPI>;
    public sealed record GetUserById(string Id) : IRequest<UserForListDto>;
    public sealed record GetUserByCompanyId(string Id) : IRequest<List<UserForListDto>>;
    public sealed record GetUserByBranchId(string Id) : IRequest<List<UserForListDto>>;
    public sealed record GetAllUser() : IRequest<List<UserForListDto>>;
}