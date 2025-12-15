using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto.Identity;
using Benzeny.Domain.Entity.Dto.User;
using BenzenyMain.Domain.Entity.Dto.User;


namespace Benzeny.Domain.IRepository
{
    public interface IUserRepository
    {
        public Task<UserInfoAPI?> GetUserInfoAsync(string id);
        public Task<ApplicationUser?> GetByIdAsync(string id);
        public Task<List<ApplicationUser>> GetAllAsync();
        Task<bool> UpdateUserAsync(UserForUpdateDto dto);
        public Task<bool> RegisterAsync(UserForRegisterDto registerRequest);
        public Task<bool> SwitchUserActiveAsync(string id);
        Task<int> CountAdminsAsync();
        Task<bool> SoftDeleteUserAsync(string userId);
        Task<GetUsersInCompany> GetAllUsersInCompanyAsync(Guid companyId);
        Task<(List<UserBenzenyDto>, int Count, int ActiveCount, int InActiveCount)> GetAllBenzenyUsersAsync(CancellationToken ct = default);
        Task<UpdateUserRolesResult> UpdateUserRolesAsync(string userId, IEnumerable<Guid> roleIds, CancellationToken ct = default);

    }
}