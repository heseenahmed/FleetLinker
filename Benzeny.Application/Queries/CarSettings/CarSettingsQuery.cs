
using BenzenyMain.Domain.Entity.Dto.Car;
using BenzenyMain.Domain.Entity.Dto.Tag;
using MediatR;

namespace BenzenyMain.Application.Queries.CarSettings
{
    public class CarSettingsQuery
    {
        #region CarBrand
        public record GetAllCarBrandsQuery() : IRequest<List<CarBrandDto>>;
        #endregion

        #region CarBrandModel
        public record GetAllCarModelsQuery() : IRequest<List<CarModelDto>>;
        #endregion

        #region PlateType
        public record GetAllPlateTypesQuery() : IRequest<List<PlateTypeDto>>;
        #endregion

        #region CarType
        public record GetAllCarTypesQuery() : IRequest<List<CarTypeDto>>;
        #endregion

        #region Tag
        public record GetAllTagsQuery() : IRequest<List<TagDto>>;
        #endregion
    }
}
