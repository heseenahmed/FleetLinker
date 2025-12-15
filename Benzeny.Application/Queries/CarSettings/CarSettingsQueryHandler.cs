using AutoMapper;
using BenzenyMain.Domain.Entity.Dto.Car;
using BenzenyMain.Domain.Entity.Dto.Tag;
using BenzenyMain.Domain.IRepository;
using MediatR;
using static BenzenyMain.Application.Queries.CarSettings.CarSettingsQuery;

namespace BenzenyMain.Application.Queries.CarSettings
{
    public class CarSettingsQueryHandler :
        IRequestHandler<GetAllCarBrandsQuery, List<CarBrandDto>>,
        IRequestHandler<GetAllCarModelsQuery, List<CarModelDto>>,
        IRequestHandler<GetAllPlateTypesQuery, List<PlateTypeDto>>,
        IRequestHandler<GetAllCarTypesQuery, List<CarTypeDto>>,
        IRequestHandler<GetAllTagsQuery, List<TagDto>>
    {
        private readonly ICarBrandRepository _carBrandRepository;
        private readonly ICarModelRepository _carModelRepository;
        private readonly IPlateTypeRepository _plateTypeRepository;
        private readonly ICarTypeRepository _carTypeRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public CarSettingsQueryHandler(
            ICarBrandRepository carBrandRepository,
            IMapper mapper,
            ICarModelRepository carModelRepository,
            IPlateTypeRepository plateTypeRepository,
            ICarTypeRepository carTypeRepository,
            ITagRepository tagRepository)
        {
            _carBrandRepository = carBrandRepository;
            _mapper = mapper;
            _carModelRepository = carModelRepository;
            _plateTypeRepository = plateTypeRepository;
            _carTypeRepository = carTypeRepository;
            _tagRepository = tagRepository;
        }

        public async Task<List<CarBrandDto>> Handle(GetAllCarBrandsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _carBrandRepository.GetListAsync();
            return _mapper.Map<List<CarBrandDto>>(entities) ?? new List<CarBrandDto>();
        }

        public async Task<List<CarModelDto>> Handle(GetAllCarModelsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _carModelRepository.GetListAsync();
            return _mapper.Map<List<CarModelDto>>(entities) ?? new List<CarModelDto>();
        }

        public async Task<List<PlateTypeDto>> Handle(GetAllPlateTypesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _plateTypeRepository.GetListAsync();
            return _mapper.Map<List<PlateTypeDto>>(entities) ?? new List<PlateTypeDto>();
        }

        public async Task<List<CarTypeDto>> Handle(GetAllCarTypesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _carTypeRepository.GetListAsync();
            return _mapper.Map<List<CarTypeDto>>(entities) ?? new List<CarTypeDto>();
        }

        public async Task<List<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _tagRepository.GetListAsync();
            return _mapper.Map<List<TagDto>>(entities) ?? new List<TagDto>();
        }
    }
}
