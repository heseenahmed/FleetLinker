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
using FleetLinker.Application.Common.Localization;
using FleetLinker.API.Resources;

namespace FleetLinker.Application.Command.User
{
    public class UserCommandHandler :
        IRequestHandler<UpdateUserAsyncCommand, bool>,
        IRequestHandler<GetPrincipalFromExpiredTokenCommand, ClaimsPrincipal>,
        IRequestHandler<RegisterCommand, bool>,
        IRequestHandler<SwitchUserActiveCommand, bool>,
        IRequestHandler<DeleteUserCommand, bool>,
        IRequestHandler<LoginCommand, APIResponse<LoginResponseDto>>,
        IRequestHandler<UpdateUserRolesCommand, UpdateUserRolesResult>
    {
        #region Fields

        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IMediator _mediator;
        private readonly IAppLocalizer _localizer;
        #endregion

        #region Constructor

        public UserCommandHandler(
            ITokenRepository tokenRepository,
            IUserRepository userRepository,
            IMailRepository mailRepository,
            IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager,
            JwtSettings jwtSettings,
            IMediator mediator
            , IAppLocalizer localizer)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _mailRepository = mailRepository;
            _env = env;
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _mediator = mediator;
            _localizer = localizer;
        }

        #endregion

        #region Update User

        public async Task<bool> Handle(UpdateUserAsyncCommand request, CancellationToken cancellationToken)
        {
            if (request.UpdateUserDto == null)
                throw new ArgumentException(_localizer[LocalizationMessages.UserUpdateDataRequired]);

            var performer = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == request.PerformedBy || u.UserName == request.PerformedBy, cancellationToken);

            if (performer == null)
                throw new UnauthorizedAccessException(_localizer[LocalizationMessages.PerformerNotFound]);

            var targetUser = await _userManager.FindByIdAsync(request.UpdateUserDto.Id);
            if (targetUser == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.TargetUserNotFound]);

            var updated = await _userRepository.UpdateUserAsync(request.UpdateUserDto);
            if (!updated)
                throw new InvalidOperationException(_localizer[LocalizationMessages.FailedToUpdateUser]);

            return true;
        }

        #endregion

        #region Token Management

        public async Task<ClaimsPrincipal> Handle(GetPrincipalFromExpiredTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                throw new ArgumentException(_localizer[LocalizationMessages.TokenRequired]);

            var principal = await _tokenRepository.GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
                throw new UnauthorizedAccessException(_localizer[LocalizationMessages.InvalidOrExpiredToken]);

            return principal;
        }

        #endregion

        #region User Registration

        public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
        
        {
            if (request.userDto == null)
                throw new ArgumentException(_localizer[LocalizationMessages.RegistrationPayloadRequired]);

            var success = await _userRepository.RegisterAsync(request.userDto);
            if (!success)
                throw new ApplicationException(_localizer[LocalizationMessages.RegistrationFailedInternally]);

            //var templatePath = Path.Combine(_env.WebRootPath, "Template", "WelcomeEmail.html");
            //if (!File.Exists(templatePath))
            //    throw new FileNotFoundException("Email template not found.", templatePath);

            //var mailBody = await File.ReadAllTextAsync(templatePath, cancellationToken);
            //mailBody = mailBody.Replace("[userName]", request.userDto.FullName)
            //                   .Replace("[resetPasswordUrl]", "https://fleetlinker.com/reset-password");

            //if (!string.IsNullOrWhiteSpace(request.userDto.Email))
            //    await _mailRepository.SendEmailAsync(request.userDto.Email, "Welcome to FleetLinker", mailBody);

            return true;
        }

        #endregion

        #region User Activation

        public async Task<bool> Handle(SwitchUserActiveCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new ArgumentException(_localizer[LocalizationMessages.UserIdRequired]);

            var targetUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.Id.ToString(), cancellationToken);
            if (targetUser == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.UserNotFound]);

            var newStatus = await _userRepository.SwitchUserActiveAsync(request.Id.ToString());
            return newStatus;
        }

        #endregion

        #region Login

        public async Task<APIResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (request.LoginRequest == null)
                throw new ArgumentException(_localizer[LocalizationMessages.LoginPayloadRequired]);

            if (string.IsNullOrWhiteSpace(request.LoginRequest.Username) ||
                string.IsNullOrWhiteSpace(request.LoginRequest.Password))
                throw new ArgumentException(_localizer[LocalizationMessages.UsernameAndPasswordRequired]);

            var username = request.LoginRequest.Username;
            var user = await _userManager.FindByNameAsync(username)
                       ?? await _userManager.FindByEmailAsync(username)
                       ?? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == username, cancellationToken);

            if (user == null || user.IsDeleted || !user.IsActive)
                throw new ArgumentException(_localizer[LocalizationMessages.UserNotFound]);

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.LoginRequest.Password);
            if (!isPasswordValid)
                throw new ArgumentException(_localizer[LocalizationMessages.InvalidPassword]);

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = TokenGenerator.GenerateAccessToken(user, roles, _jwtSettings);
            var refreshToken = TokenGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryUTC = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var response = new LoginResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                RefreshToken = user.RefreshToken,
                AccessToken = accessToken,
                Roles = roles.ToList(),
                FirstTimeLogin = user.FirstTimeLogin,
            };

            return APIResponse<LoginResponseDto>.Success(response, _localizer[LocalizationMessages.LoginSuccessful]);
        }

        #endregion

        #region Delete User

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                throw new ArgumentException(_localizer[LocalizationMessages.UserIdRequired]);

            var targetUser = await _userManager.FindByIdAsync(request.UserId);
            if (targetUser == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.UserNotFound]);

            var deleted = await _userRepository.SoftDeleteUserAsync(request.UserId);
            if (!deleted)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.UserNotFound]);

            return true;
        }

        #endregion

        #region Update User Roles

        public async Task<UpdateUserRolesResult> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                throw new ArgumentException(_localizer[LocalizationMessages.UserIdRequired]);

            if (request.RoleIds is null)
                throw new ArgumentException(_localizer[LocalizationMessages.RoleIdsCannotBeNull]);

            var targetUser = await _userManager.FindByIdAsync(request.UserId);
            if (targetUser == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.TargetUserNotFound]);

            var result = await _userRepository.UpdateUserRolesAsync(request.UserId, request.RoleIds, cancellationToken);
            return result;
        }

        #endregion
    }
}
