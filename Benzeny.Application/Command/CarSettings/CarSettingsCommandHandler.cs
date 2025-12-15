using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.IRepository;
using MediatR;

namespace BenzenyMain.Application.Command.CarSettings
{
    public class CarSettingsCommandHandler :
        IRequestHandler<CreateCarBrandCommand, bool>,
        IRequestHandler<UpdateCarBrandCommand, bool>,
        IRequestHandler<DeleteCarBrandCommand, bool>,
        IRequestHandler<CreateCarModelCommand, int>,
        IRequestHandler<UpdateCarModelCommand, Unit>,
        IRequestHandler<DeleteCarModelCommand, Unit>,
        IRequestHandler<CreatePlateTypeCommand, int>,
        IRequestHandler<UpdatePlateTypeCommand, Unit>,
        IRequestHandler<DeletePlateTypeCommand, Unit>,
        IRequestHandler<CreateCarTypeCommand, int>,
        IRequestHandler<UpdateCarTypeCommand, Unit>,
        IRequestHandler<DeleteCarTypeCommand, Unit>,
        IRequestHandler<CreateTagCommand, int>,
        IRequestHandler<UpdateTagCommand, Unit>,
        IRequestHandler<DeleteTagCommand, Unit>
    {
        private readonly ICarBrandRepository _carBrandRepository;
        private readonly ICarModelRepository _carModelRepository;
        private readonly IPlateTypeRepository _plateTypeRepository;
        private readonly ICarTypeRepository _carTypeRepository;
        private readonly ITagRepository _tagRepository;

        public CarSettingsCommandHandler(
            ICarBrandRepository carBrandRepository,
            ICarModelRepository carModelRepository,
            IPlateTypeRepository plateTypeRepository,
            ICarTypeRepository carTypeRepository,
            ITagRepository tagRepository)
        {
            _carBrandRepository = carBrandRepository;
            _carModelRepository = carModelRepository;
            _plateTypeRepository = plateTypeRepository;
            _carTypeRepository = carTypeRepository;
            _tagRepository = tagRepository;
        }

        #region CarBrand

        public async Task<bool> Handle(CreateCarBrandCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Brand title is required.");

            var entity = new CarBrand { Title = request.Title };
            var affected = await _carBrandRepository.AddAsync(entity);
            if (affected <= 0)
                throw new ApplicationException("Failed to create car brand.");

            return true;
        }

        public async Task<bool> Handle(UpdateCarBrandCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid car brand ID.");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Brand title is required.");

            var entity = await _carBrandRepository.GetByGuidAsync(request.Id)
                         ?? throw new KeyNotFoundException("CarBrand not found.");

            entity.Title = request.Title;

            var affected = _carBrandRepository.Update(entity);
            if (affected <= 0)
                throw new InvalidOperationException("Updating car brand failed.");

            return true;
        }

        public async Task<bool> Handle(DeleteCarBrandCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid car brand ID.");

            var entity = await _carBrandRepository.GetByGuidAsync(request.Id)
                         ?? throw new KeyNotFoundException("CarBrand not found.");

            var affected = await _carBrandRepository.RemoveAsync(entity);
            if (affected <= 0)
                throw new InvalidOperationException("Deleting car brand failed.");

            return true;
        }

        #endregion

        #region CarModel

        public async Task<int> Handle(CreateCarModelCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Model title is required.");

            var entity = new CarModel { Title = request.Title };
            await _carModelRepository.AddAsync(entity);
            return entity.Id;
        }

        public async Task<Unit> Handle(UpdateCarModelCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid car model ID.");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Model title is required.");

            var entity = await _carModelRepository.GetByGuidAsync(request.Id)
                         ?? throw new KeyNotFoundException("CarModel not found.");

            entity.Title = request.Title;
            _carModelRepository.Update(entity);
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteCarModelCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid car model ID.");

            var entity = await _carModelRepository.GetByGuidAsync(request.Id)
                         ?? throw new KeyNotFoundException("CarModel not found.");

            await _carModelRepository.RemoveAsync(entity);
            return Unit.Value;
        }

        #endregion

        #region PlateType

        public async Task<int> Handle(CreatePlateTypeCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Plate type title is required.");

            var entity = new PlateType { Title = request.Title };
            await _plateTypeRepository.AddAsync(entity);
            return entity.Id;
        }

        public async Task<Unit> Handle(UpdatePlateTypeCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid plate type ID.");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Plate type title is required.");

            var entity = await _plateTypeRepository.GetByGuidAsync(request.Id)
                         ?? throw new KeyNotFoundException("PlateType not found.");

            entity.Title = request.Title;
            _plateTypeRepository.Update(entity);
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeletePlateTypeCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid plate type ID.");

            var entity = await _plateTypeRepository.GetByGuidAsync(request.Id)
                         ?? throw new KeyNotFoundException("PlateType not found.");

            await _plateTypeRepository.RemoveAsync(entity);
            return Unit.Value;
        }

        #endregion

        #region CarType

        public async Task<int> Handle(CreateCarTypeCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Car type title is required.");

            var entity = new CarType { Title = request.Title };
            await _carTypeRepository.AddAsync(entity);
            return entity.Id;
        }

        public async Task<Unit> Handle(UpdateCarTypeCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid car type ID.");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Car type title is required.");

            var entity = await _carTypeRepository.GetByGuidAsync(request.Id)
                         ?? throw new KeyNotFoundException("CarType not found.");

            entity.Title = request.Title;
            _carTypeRepository.Update(entity);
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteCarTypeCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid car type ID.");

            var entity = await _carTypeRepository.GetByGuidAsync(request.Id)
                         ?? throw new KeyNotFoundException("CarType not found.");

            await _carTypeRepository.RemoveAsync(entity);
            return Unit.Value;
        }

        #endregion

        #region Tag

        public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Tag title is required.");

            var entity = new Tag { Title = request.Title };
            await _tagRepository.AddAsync(entity);
            return entity.Id;
        }

        public async Task<Unit> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid tag ID.");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Tag title is required.");

            var entity = await _tagRepository.GetByGuidAsync(request.Id)
                         ?? throw new KeyNotFoundException("Tag not found.");

            entity.Title = request.Title;
            _tagRepository.Update(entity);
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Invalid tag ID.");

            var entity = await _tagRepository.GetByGuidAsync(request.Id)
                         ?? throw new KeyNotFoundException("Tag not found.");

            await _tagRepository.RemoveAsync(entity);
            return Unit.Value;
        }

        #endregion
    }
}
