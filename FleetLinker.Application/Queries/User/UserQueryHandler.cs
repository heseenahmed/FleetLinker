using AutoMapper;
using FleetLinker.Domain.Entity.Dto.Identity;
using FleetLinker.Domain.IRepository;
using FleetLinker.Domain.Entity.Dto.User;
using MediatR;

namespace FleetLinker.Application.Queries.User
{
    public class UserCommandHandler :
        IRequestHandler<GetUserInfoAsyncCommand, UserInfoAPI?>,
        IRequestHandler<GetUserById, UserForListDto>,
        IRequestHandler<GetAllUser, List<UserForListDto>>
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

    }
      
}

