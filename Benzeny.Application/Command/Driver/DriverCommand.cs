using BenzenyMain.Domain.Entity.Dto.Driver;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BenzenyMain.Application.Command.Driver
{
    public class CreateDriverCommand : IRequest<bool>
    {
        public DriverForCreateDto DriverDto { get; }

        public CreateDriverCommand(DriverForCreateDto dto)
        {
            DriverDto = dto;
        }
    }
    public class DeleteDriverCommand : IRequest<bool>
    {
        public Guid DriverId { get; }

        public DeleteDriverCommand(Guid driverId)
        {
            DriverId = driverId;
        }
    }
    public class AssignDriverToCarCommand : IRequest<bool>
    {
        public CarDriverDto CarDriverDto { get; }

        public AssignDriverToCarCommand(CarDriverDto carDriverDto)
        {
            CarDriverDto = carDriverDto;
        }
    }
    public class UnAssignDriverToCarCommand : IRequest<bool>
    {
        public CarDriverDto CarDriverDto { get; }

        public UnAssignDriverToCarCommand(CarDriverDto carDriverDto)
        {
            CarDriverDto = carDriverDto;
        }
    }
    public class ImportDriversFromExcelCommand : IRequest<bool>
    {
        public IFormFile ExcelFile { get; }
        public string UserId { get; }
        public Guid BranchId { get; }

        public ImportDriversFromExcelCommand(IFormFile excelFile, string userId, Guid branchId)
        {
            ExcelFile = excelFile;
            UserId = userId;
            BranchId = branchId;
        }
    }
    public class DriverSwitchActiveCommand : IRequest<bool>
    {
        public Guid DriverId { get; }

        public DriverSwitchActiveCommand(Guid driverId)
        {
            DriverId = driverId;
        }
    }
    public class AssignDriverFundsCommand : IRequest<bool>
    {
        public AssignDriverFundsRequestDto Request { get; }

        public AssignDriverFundsCommand(AssignDriverFundsRequestDto request)
        {
            Request = request;
        }
    }
}
