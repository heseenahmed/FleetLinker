using AutoMapper;
using BenzenyMain.Domain.Entity.Dto.Region;
using BenzenyMain.Domain.IRepository;
using MediatR;

namespace BenzenyMain.Application.Command.Region
{
    public class RegionCommandHandler :
        IRequestHandler<CreateRegionCommand, bool>,
        IRequestHandler<UpdateRegionCommand, bool>,
        IRequestHandler<DeleteRegionCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IRegionRepository _regionRepository;

        public RegionCommandHandler(IRegionRepository regionRepository, IMapper mapper)
        {
            _mapper = mapper;
            _regionRepository = regionRepository;
        }

        public async Task<bool> Handle(CreateRegionCommand request, CancellationToken cancellationToken)
        {
            if (request?.regionDto == null)
                throw new ArgumentException("Region payload is required.");

            if (string.IsNullOrWhiteSpace(request.regionDto.Title))
                throw new ArgumentException("Region title is required.");

            var region = new Domain.Entity.Region
            {
                Title = request.regionDto.Title
            };

            await _regionRepository.AddAsync(region);
            return true;
        }

        public async Task<bool> Handle(UpdateRegionCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new ArgumentException("Invalid region ID.");

            if (request.regionDto == null)
                throw new ArgumentException("Region update payload is required.");

            var region = await _regionRepository.GetByGuidAsync(request.Id)
                         ?? throw new KeyNotFoundException("Region not found.");

            region.Title = request.regionDto.Title ?? region.Title;

            await _regionRepository.UpdateAsync(region);
            return true;
        }

        public async Task<bool> Handle(DeleteRegionCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new ArgumentException("Invalid region ID.");

            var region = await _regionRepository.GetByGuidAsync(request.Id)
                         ?? throw new KeyNotFoundException("Region not found.");

            await _regionRepository.DeleteHardAsync(region);
            return true;
        }
    }
}
