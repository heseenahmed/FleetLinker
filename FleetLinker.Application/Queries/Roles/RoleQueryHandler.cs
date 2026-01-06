using AutoMapper;
using FleetLinker.Domain.Entity;
using FleetLinker.Application.DTOs.Identity;
using FleetLinker.Domain.IRepository;
using FleetLinker.Application.Common.Localization;
using MediatR;
using System.Linq;
namespace FleetLinker.Application.Queries.Roles.Queries
{
    public class GetRoleQueryHandler : IRequestHandler<GetRoleList, IEnumerable<ApplicationRole>>
    {
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IAppLocalizer _localizer;
        public GetRoleQueryHandler(IRoleRepository repository, IMapper mapper, IAppLocalizer localizer)
        {
            _roleRepository = repository;
            _mapper = mapper;
            _localizer = localizer;
        }
        public async Task<IEnumerable<ApplicationRole>> Handle(GetRoleList request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetRolesAsync();
            if (roles == null) return Enumerable.Empty<ApplicationRole>();

            var roleList = roles.ToList();
            foreach (var role in roleList)
            {
                if (!string.IsNullOrEmpty(role.Name))
                {
                    role.Name = _localizer[LocalizationMessages.GetRoleKey(role.Name)];
                }
            }

            return roleList;
        }
    }
}
