using Benzeny.Domain.Entity.Dto.Identity;
using Benzeny.Domain.Entity.Dto.User;
using BenzenyMain.Domain.Entity.Dto.User;
using MediatR;

namespace Benzeny.Application.Queries.User
{
    public sealed record GetUserInfoAsyncCommand(string Id) : IRequest<UserInfoAPI>;
    public sealed record GetUserById(string Id) : IRequest<UserForListDto>;
    public sealed record GetUserByCompanyId(string Id) : IRequest<List<UserForListDto>>;
    public sealed record GetUserByBranchId(string Id) : IRequest<List<UserForListDto>>;
    public sealed record GetAllUser() : IRequest<List<UserForListDto>>;
    public sealed record GetAdminsCount() : IRequest<AdminsCount>;
    public class GetUsersBranch : IRequest<List<UserBranchDto>> 
    {
        public string UserId { get; set; }
        public GetUsersBranch(string userId)
        {
            UserId=userId;
        }
    }
    public class GetAllUsersInCompanyQuery : IRequest<GetUsersInCompany>
    {
        public Guid CompanyId { get; set; }

        public GetAllUsersInCompanyQuery(Guid companyId)
        {
            CompanyId = companyId;
        }
    }
    //public sealed record GetAllBenzenyUsersQuery() : IRequest<(List<UserBenzenyDto> , int count , int ActiveCount , int InActiveCount)>;
    public sealed record GetAllBenzenyUsersQuery : IRequest<GetAllBenzenyUsersResult>;

    public sealed record GetAllBenzenyUsersResult(
        List<UserBenzenyDto> Users,
        int Count,
        int ActiveCount,
        int InActiveCount
    );
}