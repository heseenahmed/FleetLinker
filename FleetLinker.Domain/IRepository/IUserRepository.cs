using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Entity.Dto.Identity;
using FleetLinker.Domain.Entity.Dto.User;
namespace FleetLinker.Domain.IRepository
{
    public interface IUserRepository
    {
        public Task<UserInfoAPI?> GetUserInfoAsync(string id);
        public Task<ApplicationUser?> GetByIdAsync(string id);
        public Task<List<ApplicationUser>> GetAllAsync();
        Task<bool> UpdateUserAsync(UserForUpdateDto dto);
        public Task<bool> RegisterAsync(UserForRegisterDto registerRequest);
        public Task<bool> SwitchUserActiveAsync(string id);
        Task<bool> SoftDeleteUserAsync(string userId);
        Task<UpdateUserRolesResult> UpdateUserRolesAsync(string userId, IEnumerable<Guid> roleIds, CancellationToken ct = default);
    }
}