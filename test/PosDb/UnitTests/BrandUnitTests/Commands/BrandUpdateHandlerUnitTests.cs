using Application.Features.Brands;
using Application.Features.Brands.UseCases.Commands.Update;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Domain.Entities.Brands;

namespace BrandUnitTests.Commands
{
    public class BrandUpdateHandlerUnitTests
    {
        private readonly Mock<IPosDbUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<ILoggingMessagesService<BrandUpdateHandler>> _mockLogger = new();
        private readonly Mock<IMapper> _mockMapper = new();

        private readonly BrandUpdateHandler _handler;
        private readonly Mock<IBrandRepository> _mockBrandRepository = new();

        public BrandUpdateHandlerUnitTests()
        {
            _mockUnitOfWork.Setup(u => u.BrandRepository).Returns(_mockBrandRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _handler = new BrandUpdateHandler(_mockLogger.Object, _mockMapper.Object, _mockUnitOfWork.Object);
        }

        ///* 
        // * El Escenario
        // * Lo que debe regresar
        // */

        [Fact]
        public async Task WhenAllIsOk_ShouldReturnUpdated()
        {
            // Arrange
            var name = "Test Brand";
            var description = "Test Description";
            var command = new BrandUpdateCommand()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description
            };
            var brand = new Brand
            {
                Id = command.Id.Value,
                Name = "Old Name",
                Description = "Old Description"
            };
            _mockBrandRepository.Setup(r => r.GetById(command.Id.Value)).ReturnsAsync(brand);
            _mockMapper
                .Setup(m => m.Map(It.IsAny<BrandUpdateCommand>(), It.IsAny<Brand>()))
                .Callback((BrandUpdateCommand cmd, Brand b) =>
                {
                    b.Name = cmd.Name;
                    b.Description = cmd.Description;
                });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Value.Should().Be(Unit.Value);
            result.ErrorDetails.Should().BeNull();

            // Verify
            _mockBrandRepository.Verify(r => r.GetById(command.Id.Value), Times.Once);
            _mockBrandRepository.Verify(r => r.Update(brand), Times.Once);
            _mockMapper.Verify(m => m.Map(command, brand), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.Updated, name, LogLevel.Information), Times.Once);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.NotFound, It.IsAny<string>(), LogLevel.Warning), Times.Never);
            _mockLogger.Verify(l => l.HandleExceptionMessage(BrandCachedKeys.ErrorUpdating, It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task WhenBrandNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var command = new BrandUpdateCommand()
            {
                Id = Guid.NewGuid(),
                Name = "Non Existing Brand",
                Description = "Some Description"
            };

            _mockBrandRepository.Setup(r => r.GetById(command.Id.Value)).ReturnsAsync((Brand?)null);

            var expectedMessage = $"Brand with name '{command.Name}' not found.";
            _mockLogger.Setup(l => l.Handle(BrandCachedKeys.NotFound, command.Name, LogLevel.Warning))
                       .ReturnsAsync(expectedMessage);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(HttpStatusCode.NotFound);
            result.Value.Should().Be(Unit.Value);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails!.Title.Should().Be(nameof(HttpStatusCode.NotFound));
            result.ErrorDetails.Message.Should().Be(expectedMessage);
            result.ErrorDetails.Errors.Should().BeNull();

            // Verify
            _mockBrandRepository.Verify(r => r.GetById(command.Id.Value), Times.Once);
            _mockBrandRepository.Verify(r => r.Update(It.IsAny<Brand>()), Times.Never);
            _mockMapper.Verify(m => m.Map(It.IsAny<BrandUpdateCommand>(), It.IsAny<Brand>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.NotFound, command.Name, LogLevel.Warning), Times.Once);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.Updated, It.IsAny<string>(), LogLevel.Information), Times.Never);
            _mockLogger.Verify(l => l.HandleExceptionMessage(BrandCachedKeys.ErrorUpdating, It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task WhenExceptionIsThrownDuringGetById_ShouldReturnInternalServerError()
        {
            // Arrange
            var command = new BrandUpdateCommand()
            {
                Id = Guid.NewGuid(),
                Name = "Error Brand",
                Description = "Some Description"
            };
            var dbException = new Exception("Database failure");

            _mockBrandRepository.Setup(r => r.GetById(command.Id.Value)).ThrowsAsync(dbException);

            var expectedLoggedMessage = "An unexpected error occurred while updating the brand.";
            _mockLogger.Setup(l => l.HandleExceptionMessage(BrandCachedKeys.ErrorUpdating, It.IsAny<Exception>()))
                       .ReturnsAsync(expectedLoggedMessage);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Value.Should().Be(Unit.Value);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails!.Title.Should().Be(nameof(HttpStatusCode.InternalServerError));
            result.ErrorDetails.Message.Should().Be(expectedLoggedMessage);
            result.ErrorDetails.Errors.Should().BeNull();

            // Verify
            _mockBrandRepository.Verify(r => r.GetById(command.Id.Value), Times.Once);
            _mockBrandRepository.Verify(r => r.Update(It.IsAny<Brand>()), Times.Never);
            _mockMapper.Verify(m => m.Map(It.IsAny<BrandUpdateCommand>(), It.IsAny<Brand>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockLogger.Verify(l => l.HandleExceptionMessage(BrandCachedKeys.ErrorUpdating, dbException), Times.Once);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.NotFound, It.IsAny<string>(), LogLevel.Warning), Times.Never);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.Updated, It.IsAny<string>(), LogLevel.Information), Times.Never);
        }

        [Fact]
        public async Task WhenExceptionIsThrownDuringSaveChangesAsync_ShouldReturnInternalServerError()
        {
            // Arrange
            var name = "Test Brand SaveChangesFail";
            var description = "Test Description";
            var command = new BrandUpdateCommand()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description
            };
            var brand = new Brand
            {
                Id = command.Id.Value,
                Name = "Old Name",
                Description = "Old Description"
            };
            var dbException = new Exception("SaveChanges failure");

            _mockBrandRepository.Setup(r => r.GetById(command.Id.Value)).ReturnsAsync(brand);
            _mockMapper
                .Setup(m => m.Map(It.IsAny<BrandUpdateCommand>(), It.IsAny<Brand>()))
                .Callback((BrandUpdateCommand cmd, Brand b) =>
                {
                    b.Name = cmd.Name;
                    b.Description = cmd.Description;
                });
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(dbException);

            var expectedLoggedMessage = "An unexpected error occurred while updating the brand.";
            _mockLogger.Setup(l => l.HandleExceptionMessage(BrandCachedKeys.ErrorUpdating, It.IsAny<Exception>()))
                       .ReturnsAsync(expectedLoggedMessage);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Value.Should().Be(Unit.Value);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails!.Title.Should().Be(nameof(HttpStatusCode.InternalServerError));
            result.ErrorDetails.Message.Should().Be(expectedLoggedMessage);

            // Verify
            _mockBrandRepository.Verify(r => r.GetById(command.Id.Value), Times.Once);
            _mockBrandRepository.Verify(r => r.Update(brand), Times.Once);
            _mockMapper.Verify(m => m.Map(command, brand), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockLogger.Verify(l => l.HandleExceptionMessage(BrandCachedKeys.ErrorUpdating, dbException), Times.Once);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.Updated, It.IsAny<string>(), LogLevel.Information), Times.Never);
        }
    }
}
