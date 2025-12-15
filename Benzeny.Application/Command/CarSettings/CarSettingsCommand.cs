
using MediatR;

namespace BenzenyMain.Application.Command.CarSettings
{
    #region CarBrand
    public record CreateCarBrandCommand(string Title) : IRequest<bool>;
    public record UpdateCarBrandCommand(int Id, string Title) : IRequest<bool>;
    public record DeleteCarBrandCommand(int Id) : IRequest<bool>;
    #endregion

    #region CarModel
    public record CreateCarModelCommand(string Title) : IRequest<int>;
    public record UpdateCarModelCommand(int Id, string Title) : IRequest<Unit>;
    public record DeleteCarModelCommand(int Id) : IRequest<Unit>;
    #endregion

    #region PlateType
    public record CreatePlateTypeCommand(string Title) : IRequest<int>;
    public record UpdatePlateTypeCommand(int Id, string Title) : IRequest<Unit>;
    public record DeletePlateTypeCommand(int Id) : IRequest<Unit>;
    #endregion

    #region CarType
    public record CreateCarTypeCommand(string Title) : IRequest<int>;
    public record UpdateCarTypeCommand(int Id, string Title) : IRequest<Unit>;
    public record DeleteCarTypeCommand(int Id) : IRequest<Unit>;
    #endregion

    #region Tag
    public record CreateTagCommand(string Title) : IRequest<int>;
    public record UpdateTagCommand(int Id, string Title) : IRequest<Unit>;
    public record DeleteTagCommand(int Id) : IRequest<Unit>;
    #endregion
}
