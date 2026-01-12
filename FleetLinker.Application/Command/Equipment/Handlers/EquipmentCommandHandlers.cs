using FleetLinker.Application.Common;
using FleetLinker.Application.Common.Interfaces;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Enums;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FleetLinker.Application.Command.Equipment.Handlers
{
    public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, APIResponse<object?>>
    {
        private readonly IEquipmentRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppLocalizer _localizer;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateEquipmentCommandHandler(
            IEquipmentRepository repository,
            UserManager<ApplicationUser> userManager,
            IAppLocalizer localizer,
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _userManager = userManager;
            _localizer = localizer;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResponse<object?>> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.CreatedBy);
            if (user == null)
                throw new KeyNotFoundException(_localizer[LocalizationMessages.UserNotFound]);

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Equipment owner"))
            {
                throw new UnauthorizedAccessException(_localizer[LocalizationMessages.EquipmentUnauthorized]);
            }

            string? imagePath = null;
            if (request.Dto.ImageFile != null)
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                var httpRequest = _httpContextAccessor.HttpContext?.Request;
                var baseUrl = $"{httpRequest?.Scheme}://{httpRequest?.Host}";
                var savedFiles = await IFileHelper.SaveFilesAsync(new List<IFormFile> { request.Dto.ImageFile }, uploadPath, baseUrl);
                imagePath = savedFiles.FirstOrDefault();
            }

            var equipment = new Domain.Entity.Equipment
            {
                BrandAr = request.Dto.BrandAr,
                BrandEn = request.Dto.BrandEn,
                YearOfManufacture = request.Dto.YearOfManufacture!.Value,
                ChassisNumber = request.Dto.ChassisNumber,
                ModelAr = request.Dto.ModelAr,
                ModelEn = request.Dto.ModelEn,
                AssetNumber = request.Dto.AssetNumber,
                OwnerId = request.CreatedBy,
                IsForSale = request.Dto.IsForSale,
                IsForRent = request.Dto.IsForRent,
                SalePrice = request.Dto.SalePrice,
                RentPrice = request.Dto.RentPrice,
                Description = request.Dto.Description,
                ImagePath = imagePath,
                CreatedBy = request.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _repository.AddAsync(equipment);
            return APIResponse<object?>.Success(null, _localizer[LocalizationMessages.EquipmentCreatedSuccessfully]);
        }
    }

    public class UpdateEquipmentCommandHandler : IRequestHandler<UpdateEquipmentCommand, APIResponse<object?>>
    {
        private readonly IEquipmentRepository _repository;
        private readonly IAppLocalizer _localizer;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateEquipmentCommandHandler(
            IEquipmentRepository repository, 
            IAppLocalizer localizer,
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _localizer = localizer;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResponse<object?>> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
        {
            var equipment = await _repository.GetByGuidAsync(request.Dto.Id);
            if (equipment == null) throw new KeyNotFoundException(_localizer[LocalizationMessages.EquipmentNotFound]);

            if (request.Dto.ImageFile != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(equipment.ImagePath))
                {
                    IFileHelper.DeleteFiles(new List<string> { equipment.ImagePath });
                }

                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                var httpRequest = _httpContextAccessor.HttpContext?.Request;
                var baseUrl = $"{httpRequest?.Scheme}://{httpRequest?.Host}";
                var savedFiles = await IFileHelper.SaveFilesAsync(new List<IFormFile> { request.Dto.ImageFile }, uploadPath, baseUrl);
                equipment.ImagePath = savedFiles.FirstOrDefault();
            }

            equipment.BrandAr = request.Dto.BrandAr;
            equipment.BrandEn = request.Dto.BrandEn;
            equipment.YearOfManufacture = request.Dto.YearOfManufacture!.Value;
            equipment.ChassisNumber = request.Dto.ChassisNumber;
            equipment.ModelAr = request.Dto.ModelAr;
            equipment.ModelEn = request.Dto.ModelEn;
            equipment.AssetNumber = request.Dto.AssetNumber;
            equipment.IsForSale = request.Dto.IsForSale;
            equipment.IsForRent = request.Dto.IsForRent;
            equipment.SalePrice = request.Dto.SalePrice;
            equipment.RentPrice = request.Dto.RentPrice;
            equipment.Description = request.Dto.Description;
            equipment.UpdatedBy = request.UpdatedBy;
            equipment.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(equipment);
            return APIResponse<object?>.Success(null, _localizer[LocalizationMessages.EquipmentUpdatedSuccessfully]);
        }
    }

    public class DeleteEquipmentCommandHandler : IRequestHandler<DeleteEquipmentCommand, APIResponse<object?>>
    {
        private readonly IEquipmentRepository _repository;
        private readonly IAppLocalizer _localizer;

        public DeleteEquipmentCommandHandler(IEquipmentRepository repository, IAppLocalizer localizer)
        {
            _repository = repository;
            _localizer = localizer;
        }

        public async Task<APIResponse<object?>> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
        {
            var equipment = await _repository.GetByGuidAsync(request.Id);
            if (equipment == null) throw new KeyNotFoundException(_localizer[LocalizationMessages.EquipmentNotFound]);

            await _repository.RemoveAsync(equipment);
            return APIResponse<object?>.Success(null, _localizer[LocalizationMessages.EquipmentDeletedSuccessfully]);
        }
    }
}
