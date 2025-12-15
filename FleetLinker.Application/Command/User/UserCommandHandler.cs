using AutoMapper;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Entity.Dto;
using FleetLinker.Domain.IRepository;
using FleetLinker.Application.Common;
using FleetLinker.Domain.Entity.Dto.User;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FleetLinker.Application.Command.User
{
    public class UserCommandHandler :
        IRequestHandler<UpdateUserAsyncCommand, bool>,
        IRequestHandler<GetPrincipalFromExpiredTokenCommand, ClaimsPrincipal>,
        IRequestHandler<RegisterCommand, bool>,
        IRequestHandler<SwitchUserActiveCommand, bool>,
        IRequestHandler<DeleteUserCommand, bool>,
        IRequestHandler<LoginCommand, APIResponse<LoginResponseDto>> ,
        IRequestHandler<UpdateUserRolesCommand, UpdateUserRolesResult>
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IMediator _mediator;
        private static readonly string[] BenzenyRoles =
        { "Benzeny", "BSuperAdmin", "BOperationAdmin", "BFinanceAdmin" };
        public UserCommandHandler(
            ITokenRepository tokenRepository,
            IUserRepository userRepository,
            IMailRepository mailRepository,
            IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager,
            JwtSettings jwtSettings ,
            IMediator mediator)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _mailRepository = mailRepository;
            _env = env;
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _mediator = mediator;
        }

        public async Task<bool> Handle(UpdateUserAsyncCommand request, CancellationToken cancellationToken)
        {
            if (request.UpdateUserDto == null)
                throw new ArgumentException("User update data is required.");

            // ?? 1. Get performer (the logged-in user)
            var performer = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == request.PerformedBy || u.UserName == request.PerformedBy, cancellationToken);

            if (performer == null)
                throw new UnauthorizedAccessException("Performer not found in system.");

            // ?? 2. Get roles of performer
            var performerRoles = await _userManager.GetRolesAsync(performer);
            var isPerformerBenzeny = performerRoles.Any(r => BenzenyRoles.Contains(r));

            // ?? 3. Get target user (the one being updated)
            var targetUser = await _userManager.FindByIdAsync(request.UpdateUserDto.Id);
            if (targetUser == null)
                throw new KeyNotFoundException("Target user not found.");

            var targetRoles = await _userManager.GetRolesAsync(targetUser);
            var isTargetBenzeny = targetRoles.Any(r => BenzenyRoles.Contains(r));

            // ?? 4. Perform update
            var updated = await _userRepository.UpdateUserAsync(request.UpdateUserDto);
            if (!updated)
                throw new InvalidOperationException("Failed to update user.");

            // ?? 5. Log only if both are Benzeny-level users
            //if (isPerformerBenzeny && isTargetBenzeny)
            //{
            //    await _mediator.Send(new CreateLogCommand(new LogForCreateDto
            //    {
            //        Action = "UpdateUser",
            //        EntityName = "User",
            //        EntityId = /*Guid.TryParse(request.UpdateUserDto.Id, out var uid) ? uid : (Guid?)*/null,
            //        PerformedBy = request.PerformedBy,
            //        Details = $"User '{targetUser.FullName}' ({targetUser.Email}) was updated by '{performer.FullName}' ({performer.Email})."
            //    }), cancellationToken);
            //}

            return true;
        }

        public async Task<ClaimsPrincipal> Handle(GetPrincipalFromExpiredTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                throw new ArgumentException("Token is required.");

            var principal = await _tokenRepository.GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
                throw new UnauthorizedAccessException("Invalid or expired token.");

            return principal;
        }

        public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (request.userDto == null)
                throw new ArgumentException("Registration payload is required.");

            // 1?? Perform the registration
            var success = await _userRepository.RegisterAsync(request.userDto);
            if (!success)
                throw new ApplicationException("Registration failed internally.");

            // 2?? Send the welcome email (existing logic)
            var templatePath = Path.Combine(_env.WebRootPath, "Template", "WelcomeEmail.html");
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Email template not found.", templatePath);

            var mailBody = await File.ReadAllTextAsync(templatePath, cancellationToken);
            mailBody = mailBody.Replace("[userName]", request.userDto.FullName)
                               .Replace("[resetPasswordUrl]", "https://benzenysystem.netlify.app/registeruser/");

            if (!string.IsNullOrWhiteSpace(request.userDto.Email))
                await _mailRepository.SendEmailAsync(request.userDto.Email, "Welcome to our channel", mailBody);

            // 3?? Retrieve the newly created user
            var newUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == request.userDto.Email, cancellationToken);

            // If not found (edge case), skip logging
            if (newUser == null)
                return true;

            // 4?? Retrieve the performer (creator)
            var performer = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == request.PerformedBy || u.UserName == request.PerformedBy, cancellationToken);

            if (performer == null)
                return true; // no performer found, probably system or external

            // 5?? Get roles for both users
            var performerRoles = await _userManager.GetRolesAsync(performer);
            var newUserRoles = await _userManager.GetRolesAsync(newUser);

            bool performerIsBenzeny = performerRoles.Any(r => BenzenyRoles.Contains(r));
            bool newUserIsBenzeny = newUserRoles.Any(r => BenzenyRoles.Contains(r));

            // 6?? Log only if both are Benzeny-level users
            //if (performerIsBenzeny && newUserIsBenzeny)
            //{
            //    var newUserRolesList = newUserRoles.Any()
            //                            ? string.Join(", ", newUserRoles)
            //                            : "No roles assigned";
            //    await _mediator.Send(new CreateLogCommand(new LogForCreateDto
            //    {
            //        Action = "RegisterNewUser",
            //        EntityName = "User",
            //        EntityId = /*Guid.TryParse(newUser.Id, out var uid) ? uid : (Guid?)*/null,
            //        PerformedBy = request.PerformedBy,
            //        Details = $"User '{newUser.FullName}' ({newUser.Email}) was created by '{performer.FullName}' ({performer.Email})" +
            //        $"with roles: {newUserRolesList}."
            //    }), cancellationToken);
            //}

            return true;
        }

        public async Task<bool> Handle(SwitchUserActiveCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new ArgumentException("Invalid user ID.");

            // ?? 1. Get target user (the one whose status will be switched)
            var targetUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.Id.ToString(), cancellationToken);
            if (targetUser == null)
                throw new KeyNotFoundException("User not found.");

            // ?? 2. Get performer
            var performer = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == request.PerformedBy || u.UserName == request.PerformedBy, cancellationToken);

            // ?? 3. Switch user active state
            var newStatus = await _userRepository.SwitchUserActiveAsync(request.Id.ToString());

            // ?? 4. If performer missing, skip logging
            if (performer == null)
                return newStatus;

            // ?? 5. Get both roles
            var performerRoles = await _userManager.GetRolesAsync(performer);
            var targetRoles = await _userManager.GetRolesAsync(targetUser);

            bool performerIsBenzeny = performerRoles.Any(r => BenzenyRoles.Contains(r));
            bool targetIsBenzeny = targetRoles.Any(r => BenzenyRoles.Contains(r));

            // ?? 6. Log only if both users have Benzeny roles
            //if (performerIsBenzeny && targetIsBenzeny)
            //{
            //    await _mediator.Send(new CreateLogCommand(new LogForCreateDto
            //    {
            //        Action = "SwitchUserActive",
            //        EntityName = "User",
            //        EntityId = null,
            //        PerformedBy = request.PerformedBy,
            //        Details = $"User '{targetUser.FullName}' ({targetUser.Email}) was {(newStatus ? "activated" : "deactivated")} by '{performer.FullName}' ({performer.Email})."
            //    }), cancellationToken);
            //}

            return newStatus;
        }

        public async Task<APIResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (request.LoginRequest == null)
                throw new ArgumentException("Login payload is required.");

            if (string.IsNullOrWhiteSpace(request.LoginRequest.Username) ||
                string.IsNullOrWhiteSpace(request.LoginRequest.Password))
                throw new ArgumentException("Username and password are required.");

            var username = request.LoginRequest.Username;

            var user = await _userManager.FindByNameAsync(username)
                       ?? await _userManager.FindByEmailAsync(username)
                       ?? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == username, cancellationToken);

            if (user == null || user.IsDeleted || !user.IsActive)
                throw new ArgumentException("User Not Found.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.LoginRequest.Password);
            if (!isPasswordValid)
                throw new ArgumentException("Invalid password. Please try again.");

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = TokenGenerator.GenerateAccessToken(user, roles, _jwtSettings);
            var refreshToken = TokenGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryUTC = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            Guid? companyId = null;
            string? companyName = null;

            //if (user.CompanyId != null)
            //{
            //    var company = await _companyRepository.GetCompanyWithUserByIdAsync(user.CompanyId.Value);
            //    if (company != null)
            //    {
            //        companyId = company.Company.Id;
            //        companyName = company.Company.Name;
            //    }
            //}

            //var userBranches = await _branchRepository.GetBranchesForUserAsync(user.Id);

            var response = new LoginResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                RefreshToken = user.RefreshToken,
                AccessToken = accessToken,
                Roles = roles.ToList(),
                CompanyId = companyId,
                CompanyName = companyName,
                FirstTimeLogin=user.FirstTimeLogin,
            };

            return APIResponse<LoginResponseDto>.Success(response, "Login successful.");
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                throw new ArgumentException("User ID is required.");

            // ?? 1. Get target user (to be deleted)
            var targetUser = await _userManager.FindByIdAsync(request.UserId);
            if (targetUser == null)
                throw new KeyNotFoundException("User not found or already deleted.");

            // ?? 2. Get performer (the logged-in user)
            var performer = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == request.PerformedBy || u.UserName == request.PerformedBy, cancellationToken);

            // ?? 3. Perform delete first
            var deleted = await _userRepository.SoftDeleteUserAsync(request.UserId);
            if (!deleted)
                throw new KeyNotFoundException("User not found or already deleted.");

            // If performer not found (e.g. system process), stop here
            if (performer == null)
                return true;

            // ?? 4. Get roles
            var performerRoles = await _userManager.GetRolesAsync(performer);
            var targetRoles = await _userManager.GetRolesAsync(targetUser);

            bool performerIsBenzeny = performerRoles.Any(r => BenzenyRoles.Contains(r));
            bool targetIsBenzeny = targetRoles.Any(r => BenzenyRoles.Contains(r));

            // ?? 5. Log only if both are Benzeny-level users
            if (performerIsBenzeny && targetIsBenzeny)
            {
                var targetRolesList = targetRoles.Any()
                    ? string.Join(", ", targetRoles)
                    : "No roles assigned";

                //await _mediator.Send(new CreateLogCommand(new LogForCreateDto
                //{
                //    Action = "DeleteUser",
                //    EntityName = "User",
                //    EntityId = /*Guid.TryParse(targetUser.Id, out var uid) ? uid : (Guid?)*/null,
                //    PerformedBy = request.PerformedBy,
                //    Details = $"User '{targetUser.FullName}' ({targetUser.Email}) was deleted by '{performer.FullName}' ({performer.Email}) with roles: {targetRolesList}."
                //}), cancellationToken);
            }

            return true;
        }
        public async Task<UpdateUserRolesResult> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                throw new ArgumentException("UserId is required.");

            if (request.RoleIds is null)
                throw new ArgumentException("RoleIds cannot be null. Send empty list to remove all roles.");

            // ?? 1. Get target user
            var targetUser = await _userManager.FindByIdAsync(request.UserId);
            if (targetUser == null)
                throw new KeyNotFoundException("Target user not found.");

            // ?? 2. Get performer
            var performer = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == request.PerformedBy || u.UserName == request.PerformedBy, cancellationToken);

            // ?? 3. Update roles via repository
            var result = await _userRepository.UpdateUserRolesAsync(request.UserId, request.RoleIds, cancellationToken);

            // ?? 4. If performer missing (system call), just return result
            if (performer == null)
                return result;

            // ?? 5. Retrieve roles for both users
            var performerRoles = await _userManager.GetRolesAsync(performer);
            var targetRoles = await _userManager.GetRolesAsync(targetUser);

            bool performerIsBenzeny = performerRoles.Any(r => BenzenyRoles.Contains(r));
            bool targetIsBenzeny = targetRoles.Any(r => BenzenyRoles.Contains(r));

            // ?? 6. Log only if both are Benzeny-level users
            if (performerIsBenzeny && targetIsBenzeny)
            {
                var finalRolesList = result.FinalRoles?.Any() == true
                    ? string.Join(", ", result.FinalRoles.Select(r => r.Name))
                    : "No roles assigned";

                //await _mediator.Send(new CreateLogCommand(new LogForCreateDto
                //{
                //    Action = "UpdateUserRoles",
                //    EntityName = "User",
                //    EntityId = null,
                //    PerformedBy = request.PerformedBy,
                //    Details = $"User '{targetUser.FullName}' ({targetUser.Email}) roles were updated by '{performer.FullName}' ({performer.Email}). Final roles: {finalRolesList}."
                //}), cancellationToken);
            }

            return result;
        }
    }
}
