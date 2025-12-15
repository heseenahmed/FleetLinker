
using BenzenyMain.Domain.Entity.Dto.Ads;
using MediatR;

namespace BenzenyMain.Application.Command.Ads
{
    public class CreateAdsCommand : IRequest<bool>
    {
        public CreateAdsDto AdDto { get; set; }
        public string CreatedBy { get; set; }
        public CreateAdsCommand(CreateAdsDto adDto, string createdBy) 
        {
            AdDto = adDto;
            CreatedBy = createdBy;
        }
    }
    public class UpdateAdvertisementCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public UpdateAdsDto Advertisement { get; set; }
        public string UpdatedBy { get; set; }
        public UpdateAdvertisementCommand(UpdateAdsDto adDto, Guid id , string updatedBy) 
        {
            Id = id;
            Advertisement = adDto;
            UpdatedBy = updatedBy;
        }
    }
    public class HardDeleteAdsCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public HardDeleteAdsCommand(Guid id) => Id = id;
    }
    public class SwitchActiveAdsCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public SwitchActiveAdsCommand(Guid id) => Id = id;
    }
}
