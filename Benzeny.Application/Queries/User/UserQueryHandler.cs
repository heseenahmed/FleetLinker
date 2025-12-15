using AutoMapper;
using Benzeny.Domain.Entity.Dto.Identity;
using Benzeny.Domain.Entity.Dto.User;
using Benzeny.Domain.IRepository;
using BenzenyMain.Domain.Entity.Dto.User;
using MediatR;

namespace Benzeny.Application.Queries.User
{
    public class UserCommandHandler :
        IRequestHandler<GetUserInfoAsyncCommand, UserInfoAPI?>,
        IRequestHandler<GetUserById, UserForListDto>,
        IRequestHandler<GetAllUser, List<UserForListDto>>,
        IRequestHandler<GetAdminsCount, AdminsCount>,
        IRequestHandler<GetAllUsersInCompanyQuery, GetUsersInCompany> ,
        IRequestHandler<GetAllBenzenyUsersQuery, GetAllBenzenyUsersResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserInfoAPI?> Handle(GetUserInfoAsyncCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
                throw new ArgumentException("User ID cannot be empty.");

            var userInfo = await _userRepository.GetUserInfoAsync(request.Id)
                           ?? throw new KeyNotFoundException("User not found with the given ID.");

            return userInfo;
        }

        public async Task<UserForListDto> Handle(GetUserById request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
                throw new ArgumentException("User ID is required.");

            var user = await _userRepository.GetByIdAsync(request.Id)
                       ?? throw new KeyNotFoundException("User not found.");

            return _mapper.Map<UserForListDto>(user);
        }

        public async Task<List<UserForListDto>> Handle(GetAllUser request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync() ?? new List<Domain.Entity.ApplicationUser>();
            return _mapper.Map<List<UserForListDto>>(users) ?? new List<UserForListDto>();
        }

        public async Task<AdminsCount> Handle(GetAdminsCount request, CancellationToken cancellationToken)
        {
            var count = await _userRepository.CountAdminsAsync();
            return new AdminsCount { Count = count };
        }

        public async Task<GetUsersInCompany> Handle(GetAllUsersInCompanyQuery request, CancellationToken cancellationToken)
        {
            if (request.CompanyId == Guid.Empty)
                throw new ArgumentException("Company ID is invalid.");

            var result = await _userRepository.GetAllUsersInCompanyAsync(request.CompanyId)
                         ?? throw new KeyNotFoundException("No users found in this company.");

            return result;
        }
        #region Benzeny
        //public async Task<(List<UserBenzenyDto>, int count, int ActiveCount, int InActiveCount)> Handle(GetAllBenzenyUsersQuery request, CancellationToken cancellationToken)
        //{
        //    var users = await _userRepository.GetAllBenzenyUsersAsync(cancellationToken);

        //    if (users.Count == 0)
        //        throw new KeyNotFoundException("No Benzeny users found.");

        //    return users;
        //}
        public async Task<GetAllBenzenyUsersResult> Handle(GetAllBenzenyUsersQuery request, CancellationToken cancellationToken)
        {
            var (users, count, activeCount, inactiveCount) =
                await _userRepository.GetAllBenzenyUsersAsync(cancellationToken);

            if (count == 0)
                throw new KeyNotFoundException("No Benzeny users found.");


            return new GetAllBenzenyUsersResult(users, count, activeCount, inactiveCount);
        }
    }
    #endregion
}

