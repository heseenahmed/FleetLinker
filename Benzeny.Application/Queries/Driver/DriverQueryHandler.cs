using AutoMapper;
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity.Dto.Driver;
using BenzenyMain.Domain.IRepository;
using MediatR;

namespace BenzenyMain.Application.Queries.Driver
{
    public class DriverQueryHandler :
        IRequestHandler<GetAllDriversQuery, PaginatedResult<DriverForListDto>>,
        IRequestHandler<GetDriverByIdQuery, DriverForGetIdDto>,
        IRequestHandler<GetDriverInBranchQuery, PaginatedResult<DriverForListDto>>,
        IRequestHandler<GetDriversStatusInCompany, DriverStatusDto>
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IMapper _mapper;

        public DriverQueryHandler(IDriverRepository driverRepository, IMapper mapper)
        {
            _driverRepository = driverRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<DriverForListDto>> Handle(GetAllDriversQuery request, CancellationToken cancellationToken)
        {
            if (request.PageNumber < 1 || request.PageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than zero.");

            var (drivers, totalCount, activeCount, inActiveCount) =
                await _driverRepository.GetAllAsync(request.PageNumber, request.PageSize, request.SearchTerm);

            var mappedDrivers = _mapper.Map<List<DriverForListDto>>(drivers) ?? new List<DriverForListDto>();

            return new PaginatedResult<DriverForListDto>(
                mappedDrivers, totalCount, request.PageNumber, request.PageSize, activeCount, inActiveCount);
        }

        public async Task<DriverForGetIdDto> Handle(GetDriverByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.DriverId == Guid.Empty)
                throw new ArgumentException("Invalid driver ID.");

            var driver = await _driverRepository.GetByIdAsync(request.DriverId)
                         ?? throw new KeyNotFoundException("Driver not found.");

            return driver; // repo already returns the DTO
        }

        public async Task<PaginatedResult<DriverForListDto>> Handle(GetDriverInBranchQuery request, CancellationToken cancellationToken)
        {
            if (request.BranchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");
            if (request.PageNumber < 1 || request.PageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than zero.");

            var result = await _driverRepository.GetDriversInBranch(
                request.BranchId, request.PageNumber, request.PageSize, request.SearchTerm);

            var mappedDrivers = _mapper.Map<List<DriverForListDto>>(result.Items) ?? new List<DriverForListDto>();

            return new PaginatedResult<DriverForListDto>(
                mappedDrivers, result.TotalCount, result.PageNumber, result.PageSize, result.ActiveCount, result.InActiveCount);
        }

        public async Task<DriverStatusDto> Handle(GetDriversStatusInCompany request, CancellationToken cancellationToken)
        {
            if (request.CompanyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var status = await _driverRepository.GetDriversStatusInCompany(request.CompanyId)
                         ?? throw new KeyNotFoundException("Company not found or has no drivers.");

            return status;
        }
    }
}
