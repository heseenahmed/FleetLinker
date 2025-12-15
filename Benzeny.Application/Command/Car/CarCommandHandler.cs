using AutoMapper;
using BenzenyMain.Domain.IRepository;
using MediatR;

namespace BenzenyMain.Application.Command.Car
{
    public class CarCommandHandler :
        IRequestHandler<CreateCarCommand, bool>,
        IRequestHandler<UpdateCarCommand, bool>,
        IRequestHandler<DeleteCarCommand, bool>,
        IRequestHandler<ImportCarsFromExcelCommand, bool>,
        IRequestHandler<CarSwitchActiveCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly ICarRepository _carRepository;

        public CarCommandHandler(IMapper mapper, ICarRepository carRepository)
        {
            _mapper = mapper;
            _carRepository = carRepository;
        }

        public async Task<bool> Handle(CreateCarCommand request, CancellationToken cancellationToken)
        {
            if (request?.Car == null)
                throw new ArgumentException("Car payload is required.");

            var car = _mapper.Map<Domain.Entity.Car>(request.Car);
            car.CreatedBy= request.Car.CreatedBy ?? "Admin";
            car.CreatedDate= DateTime.Now;
            var created = await _carRepository.AddAsync(car);
            if (!created)
                throw new ApplicationException("Failed to create car.");

            return true;
        }

        public async Task<bool> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
        {
            if (request?.Car == null)
                throw new ArgumentException("Car payload is required.");

            if (request.Car.Id == Guid.Empty)
                throw new ArgumentException("Invalid car ID.");

            var car = _mapper.Map<Domain.Entity.Car>(request.Car);
            car.UpdatedBy= request.Car.UpdatedBy ?? "Admin";
            car.UpdatedDate= DateTime.Now;

            var updated = await _carRepository.UpdateAsync(car);
            if (!updated)
                throw new KeyNotFoundException("Car not found or update failed.");

            return true;
        }

        public async Task<bool> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
        {
            if (request.CarId == Guid.Empty)
                throw new ArgumentException("Invalid car ID.");

            var deleted = await _carRepository.DeleteAsync(request.CarId);
            if (!deleted)
                throw new KeyNotFoundException("Car not found or already deleted.");

            return true;
        }

        public async Task<bool> Handle(ImportCarsFromExcelCommand request, CancellationToken cancellationToken)
        {
            // Basic validation
            if (request.ExcelFile == null || request.ExcelFile.Length == 0)
                throw new ArgumentException("Excel file is required.");

            if (string.IsNullOrWhiteSpace(request.UserId))
                throw new ArgumentException("User ID is required.");

            if (request.BranchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");

            // Repository call
            var result = await _carRepository.ImportCarsFromExcelAsync(
                request.ExcelFile,
                request.UserId,
                request.BranchId);

            if (!result)
                throw new InvalidOperationException("Import failed. Please verify Excel format and data validity.");

            return true;
        }

        public async Task<bool> Handle(CarSwitchActiveCommand request, CancellationToken cancellationToken)
        {
            if (request.CarId == Guid.Empty)
                throw new ArgumentException("Invalid car ID.");

            var toggled = await _carRepository.CarSwitchActive(request.CarId);
            if (!toggled)
                throw new KeyNotFoundException("Car not found.");

            return true; // or `return toggled;` both are true here
        }
    }
}
