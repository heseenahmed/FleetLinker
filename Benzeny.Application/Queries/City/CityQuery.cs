
using BenzenyMain.Domain.Entity.Dto.City;
using MediatR;

namespace BenzenyMain.Application.Queries.City
{
    public class GetCitiesByRegionQuery:IRequest<List<CityDto>>
    {
        public Guid RegionId { get; set; }
        public GetCitiesByRegionQuery(Guid regionId)
        {
            RegionId = regionId;
        }
    }
}
