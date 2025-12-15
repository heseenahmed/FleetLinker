
using BenzenyMain.Domain.Entity.Dto.Car;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BenzenyMain.Application.Command.Car
{
    public class CreateCarCommand : IRequest<bool>
    {
        public CarForCreateDto Car { get; set; }
        public CreateCarCommand(CarForCreateDto car)
        {
            this.Car = car;
        }
    }

    public class UpdateCarCommand : IRequest<bool>
    {
        //public Guid Id { get; set; }
        public UpdateCarDto Car { get; set; }
        public UpdateCarCommand(UpdateCarDto car)
        {
            this.Car = car;
        }

    }

    public class DeleteCarCommand : IRequest<bool>
    {
        public Guid CarId { get; set; }

        public DeleteCarCommand(Guid carId)
        {
            CarId = carId;
        }
    }
    public class CarSwitchActiveCommand : IRequest<bool>
    {
        public Guid CarId { get; set; }

        public CarSwitchActiveCommand(Guid carId)
        {
            CarId = carId;
        }
    }

    public record ImportCarsFromExcelCommand(IFormFile ExcelFile, string UserId, Guid BranchId) : IRequest<bool>;



}
