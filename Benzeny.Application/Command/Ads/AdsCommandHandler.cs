using AutoMapper;
using Benzeny.Application.Common;
using BenzenyMain.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BenzenyMain.Application.Command.Ads
{
    public class AdsCommandHandler :
        IRequestHandler<CreateAdsCommand, bool>,
        IRequestHandler<UpdateAdvertisementCommand, bool>,
        IRequestHandler<HardDeleteAdsCommand, bool>,
        IRequestHandler<SwitchActiveAdsCommand, bool>
    {
        private readonly IAdsRepository _adsRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public AdsCommandHandler(IAdsRepository adsRepo, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _adsRepo = adsRepo;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<bool> Handle(CreateAdsCommand request, CancellationToken cancellationToken)
        {
            if (request?.AdDto == null)
                throw new ArgumentException("Request payload is required.");

            if (request.AdDto.Image == null || request.AdDto.Image.Length == 0)
                throw new ArgumentException("Image is required and must be 600x600.");

            var httpContext = _contextAccessor.HttpContext
                              ?? throw new UnauthorizedAccessException("User context not available.");

            var userId = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User not found in token.");

            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            var imagePath = await ImageHelper.SaveImageAsync(
                request.AdDto.Image,
                Path.Combine("wwwroot", "uploads"),
                baseUrl
            );

            var ad = _mapper.Map<Domain.Entity.Ads>(request.AdDto);

            // set server-side fields
            ad.Image = imagePath;
            ad.CreatedBy = userId;
            ad.UserId = userId;
            ad.IsActive = false;
            ad.CreatedDate = DateTime.UtcNow;
            ad.CreatedBy = request.CreatedBy;
            ad.StartDate = ad.CreatedDate;
            ad.EndDate = ad.CreatedDate.AddMonths(ad.DurationInMonths);

            await _adsRepo.CreateAsync(ad);
            return true;
        }

        public async Task<bool> Handle(UpdateAdvertisementCommand request, CancellationToken cancellationToken)
        {
            if (request == null || request.Advertisement == null)
                throw new ArgumentException("Request data is missing.");

            if (request.Id == Guid.Empty)
                throw new ArgumentException("Invalid advertisement ID.");

            var existingAd = await _adsRepo.GetByIdAsync(request.Id)
                            ?? throw new KeyNotFoundException("Advertisement not found.");

            var httpContext = _contextAccessor.HttpContext
                              ?? throw new UnauthorizedAccessException("User context not available.");

            // update image only if a new one is provided
            if (request.Advertisement.Image != null && request.Advertisement.Image.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(existingAd.Image))
                    ImageHelper.DeleteImage(existingAd.Image);

                var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
                var imagePath = await ImageHelper.SaveImageAsync(
                    request.Advertisement.Image,
                    Path.Combine("wwwroot", "uploads"),
                    baseUrl
                );

                existingAd.Image = imagePath;
            }

            // map other fields (profile should ignore Image)
            _mapper.Map(request.Advertisement, existingAd);

            // audit fields
            existingAd.Id = request.Id;
            existingAd.UpdatedDate = DateTime.UtcNow;
            existingAd.UpdatedBy = request.UpdatedBy;

            // recompute EndDate from StartDate if present
            if (existingAd.StartDate.HasValue)
                existingAd.EndDate = existingAd.StartDate.Value.AddMonths(request.Advertisement.DurationInMonths);

            await _adsRepo.UpdateAsync(existingAd);
            return true;
        }

        public async Task<bool> Handle(HardDeleteAdsCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new ArgumentException("Invalid advertisement ID.");

            var ad = await _adsRepo.GetByIdAsync(request.Id)
                     ?? throw new KeyNotFoundException("Advertisement not found.");

            if (!string.IsNullOrWhiteSpace(ad.Image))
                ImageHelper.DeleteImage(ad.Image);

            await _adsRepo.DeleteAsync(ad);
            return true;
        }

        public async Task<bool> Handle(SwitchActiveAdsCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                throw new ArgumentException("Invalid advertisement ID.");

            var ad = await _adsRepo.GetByIdAsync(request.Id)
                     ?? throw new KeyNotFoundException("Advertisement not found.");

            ad.IsActive = !ad.IsActive;

            await _adsRepo.UpdateAsync(ad);
            return ad.IsActive;
        }
    }
}
