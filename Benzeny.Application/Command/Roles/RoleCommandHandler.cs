using AutoMapper;
using Benzeny.Application.Command.Companies;
using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto.Identity;
using Benzeny.Domain.IRepository;
using MediatR;

namespace Benzeny.Application.Command.Roles
{
    public class UserCommandHandler :
        IRequestHandler<AddRoleCommand, bool>,
        IRequestHandler<DeleteRoleCommand, bool>,
        IRequestHandler<UpdateRoleCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public UserCommandHandler(IMapper mapper, IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<bool> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            if (request?.Role == null)
                throw new ArgumentException("Role payload is required.");

            var newRole = _mapper.Map<ApplicationRole>(request.Role);
            if (string.IsNullOrWhiteSpace(newRole?.Name))
                throw new ArgumentException("Role name is required.");

            newRole.Id = Guid.NewGuid().ToString();
            newRole.NormalizedName = newRole.Name.ToUpperInvariant();

            var created = await _roleRepository.AddRoleAsync(newRole);
            if (!created)
                throw new InvalidOperationException("Failed to add role (it may already exist).");

            return true;
        }

        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RoleName))
                throw new ArgumentException("Role name is required.");

            var deleted = await _roleRepository.DeleteRoleByNameAsync(request.RoleName);
            if (!deleted)
                throw new KeyNotFoundException($"Role '{request.RoleName}' not found.");

            return true;
        }

        public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            if (request?.Role == null)
                throw new ArgumentException("Role payload is required.");

            if (string.IsNullOrWhiteSpace(request.Role.Name))
                throw new ArgumentException("Role name is required.");

            var updated = await _roleRepository.UpdateRoleAsync(request.Role);
            if (!updated)
                throw new InvalidOperationException("Failed to update role (role may not exist).");

            return true;
        }
    }
}
