using AutoMapper;
using Benzeny.Domain.Entity;
using BenzenyMain.Domain.IRepository;
using MediatR;

namespace BenzenyMain.Application.Command.Driver
{
    public class DriverCommandHandler :
        IRequestHandler<CreateDriverCommand, bool>,
        IRequestHandler<DeleteDriverCommand, bool>,
        IRequestHandler<AssignDriverToCarCommand, bool>,
        IRequestHandler<UnAssignDriverToCarCommand, bool>,
        IRequestHandler<ImportDriversFromExcelCommand, bool>,
        IRequestHandler<DriverSwitchActiveCommand, bool>,
        IRequestHandler<AssignDriverFundsCommand, bool>
    {
        private readonly IDriverRepository _driverRepo;
        private readonly IMapper _mapper;

        public DriverCommandHandler(IDriverRepository driverRepo, IMapper mapper)
        {
            _driverRepo = driverRepo;
            _mapper = mapper;
        }

        public async Task<bool> Handle(CreateDriverCommand request, CancellationToken cancellationToken)
        {
            var dto = request?.DriverDto ?? throw new ArgumentException("Driver payload is required.");

            // Build user
            var user = new ApplicationUser
            {
                UserName = dto.phoneNumber,
                FullName = dto.FullName,
                PhoneNumber = dto.phoneNumber
            };

            // Build driver
            var driver = new Domain.Entity.Driver
            {
                BranchId = dto.branchId,
                License = dto.License,
                LicenseDegree = dto.LicenseDegree,
                TagId = dto.TagId,
                CreatedBy = dto.CreatedBy ?? "Admin",
                CreatedDate = dto.CreatedDate
            };

            var success = await _driverRepo.CreateDriverWithUserAsync(driver, user);
            if (!success)
                throw new ApplicationException("Failed to create driver.");

            return true;
        }

        public async Task<bool> Handle(DeleteDriverCommand request, CancellationToken cancellationToken)
        {
            if (request.DriverId == Guid.Empty)
                throw new ArgumentException("Invalid driver ID.");

            var deleted = await _driverRepo.DeleteAsync(request.DriverId);
            if (!deleted)
                throw new KeyNotFoundException("Driver not found or already deleted.");

            return true;
        }

        public async Task<bool> Handle(AssignDriverToCarCommand request, CancellationToken cancellationToken)
        {
            if (request.CarDriverDto == null)
                throw new ArgumentException("Body is required.");

            var assigned = await _driverRepo.AssignDriverToCarAsync(request.CarDriverDto);
            if (!assigned)
                throw new InvalidOperationException("Failed to assign driver to car.");

            return true;
        }

        public async Task<bool> Handle(UnAssignDriverToCarCommand request, CancellationToken cancellationToken)
        {
            if (request.CarDriverDto == null)
                throw new ArgumentException("Body is required.");

            var unassigned = await _driverRepo.UnassignDriverFromCarAsync(request.CarDriverDto);
            if (!unassigned)
                throw new KeyNotFoundException("Driver is not assigned to this car or unassignment failed.");

            return true;
        }

        public async Task<bool> Handle(ImportDriversFromExcelCommand request, CancellationToken cancellationToken)
        {
            if (request.ExcelFile == null || request.ExcelFile.Length == 0)
                throw new ArgumentException("Excel file is required.");

            if (string.IsNullOrWhiteSpace(request.UserId))
                throw new ArgumentException("User ID is required.");

            if (request.BranchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");

            var imported = await _driverRepo.ImportDriverFromExcelAsync(request.ExcelFile, request.UserId, request.BranchId);
            if (!imported)
                throw new InvalidOperationException("Drivers import failed.");

            return true;
        }


        public async Task<bool> Handle(DriverSwitchActiveCommand request, CancellationToken cancellationToken)
        {
            if (request.DriverId == Guid.Empty)
                throw new ArgumentException("Invalid driver ID.");

            var toggled = await _driverRepo.DriverSwitchActive(request.DriverId);
            if (!toggled)
                throw new KeyNotFoundException("Driver not found.");

            return true; // toggled is true here
        }

        public async Task<bool> Handle(AssignDriverFundsCommand request, CancellationToken cancellationToken)
        {
            if (request.Request == null)
                throw new ArgumentException("Body is required.");

            var ok = await _driverRepo.AssignAmountAndTransactionTypeToDriversAsync(request.Request);
            if (!ok)
                throw new InvalidOperationException("Assigning funds to drivers failed.");

            return true;
        }
    }
}
