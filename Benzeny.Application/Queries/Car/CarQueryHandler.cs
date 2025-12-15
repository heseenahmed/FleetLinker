using AutoMapper;
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity.Dto.Car;
using BenzenyMain.Domain.IRepository;
using MediatR;

namespace BenzenyMain.Application.Queries.Car
{
    public class CarQueryHandler :
        IRequestHandler<GetAllCarsQuery, PaginatedResult<CarForGet>>,
        IRequestHandler<GetCarByIdQuery, CarForListDto>,
        IRequestHandler<GetCarsInBranchQuery, PaginatedResult<CarForGet>>,
        IRequestHandler<GetCarsInCompanyQuery, CarCompanyDto>
    {
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;

        public CarQueryHandler(ICarRepository carRepository, IMapper mapper)
        {
            _carRepository = carRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<CarForGet>> Handle(GetAllCarsQuery request, CancellationToken cancellationToken)
        {
            if (request.PageNumber < 1 || request.PageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than zero.");

            return await _carRepository.GetAllAsync(request.PageNumber, request.PageSize, request.SearchTerm);
        }

        public async Task<CarForListDto> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.CarId == Guid.Empty)
                throw new ArgumentException("Invalid car ID.");

            var car = await _carRepository.GetByIdAsync(request.CarId)
                      ?? throw new KeyNotFoundException("Car not found.");

            return _mapper.Map<CarForListDto>(car);
        }

        public async Task<PaginatedResult<CarForGet>> Handle(GetCarsInBranchQuery request, CancellationToken cancellationToken)
        {
            if (request.BranchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");
            if (request.PageNumber < 1 || request.PageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than zero.");

            return await _carRepository.GetCarsInBranch(request.BranchId, request.PageNumber, request.PageSize, request.Search);
        }

        public async Task<CarCompanyDto> Handle(GetCarsInCompanyQuery request, CancellationToken cancellationToken)
        {
            if (request.CompanyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var dto = await _carRepository.GetCompanyCarStatusCountsAsync(request.CompanyId);
            if (dto == null)
                throw new KeyNotFoundException("Company not found.");

            return dto;
        }
    }
}
