using AutoMapper;
using BenzenyMain.Domain.Entity.Dto.City;
using BenzenyMain.Domain.IRepository;
using MediatR;

namespace BenzenyMain.Application.Queries.City
{
    public class CityQueryHandler : IRequestHandler<GetCitiesByRegionQuery, List<CityDto>>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public CityQueryHandler(ICityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        public async Task<List<CityDto>> Handle(GetCitiesByRegionQuery request, CancellationToken cancellationToken)
        {
            if (request.RegionId == Guid.Empty)
                throw new ArgumentException("Region ID cannot be empty.");

            var result = await _cityRepository.GetCitiesByRegionIdAsync(request.RegionId);

            if (result == null || !result.Any())
                throw new KeyNotFoundException("No cities found for the given region.");

            return _mapper.Map<List<CityDto>>(result);
        }
    }
}
