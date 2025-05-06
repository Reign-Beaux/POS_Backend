using Application.Features.Brands;
using Application.Features.Brands.UseCases.Commands.Create;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Domain.Entities.Brands;

namespace BrandUnitTests.Commands
{
    public class BrandCreateHandlerUnitTests    
    {
        private readonly Mock<IPosDbUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<ILoggingMessagesService<BrandCreateHandler>> _mockLogger = new();
        private readonly Mock<IMapper> _mockMapper = new();

        private readonly BrandCreateHandler _handler;
        private readonly Mock<IBrandRepository> _mockBrandRepository = new();

        public BrandCreateHandlerUnitTests()
        {
            _mockUnitOfWork.Setup(u => u.BrandRepository).Returns(_mockBrandRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _handler = new BrandCreateHandler(_mockLogger.Object, _mockMapper.Object, _mockUnitOfWork.Object);
        }

        ///* 
        // * El Escenario
        // * Lo que debe regresar
        // */

        [Fact]
        public async Task WhenAllIsOk_ShouldReturnCreated()
        {
            // Arrange
            var name = "Test Brand";
            var description = "Test Description";
            var command = new BrandCreateCommand()
            {
                Name = name,
                Description = description
            };

            _mockBrandRepository.Setup(r => r.GetByName(name)).ReturnsAsync((Brand?)null);

            _mockMapper
                .Setup(m => m.Map<Brand>(It.IsAny<BrandCreateCommand>()))
                .Returns((BrandCreateCommand cmd) => new Brand
                {
                    Id = Guid.NewGuid(),
                    Name = cmd.Name,
                    Description = cmd.Description
                });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(HttpStatusCode.Created);
            result.Value.Should().NotBeEmpty();
            result.ErrorDetails.Should().BeNull();

            // Verify
            _mockBrandRepository.Verify(r => r.GetByName(name), Times.Once);
            _mockBrandRepository.Verify(r => r.Add(It.IsAny<Brand>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockLogger.Verify(l => l.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, It.IsAny<Exception>()), Times.Never);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.CreatedSuccessfully, LogLevel.Information), Times.Once);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.AlreadyExists, name, LogLevel.Warning), Times.Never);
        }

        [Fact]
        public async Task WhenBrandAlreadyExists_ShouldReturnConflict()
        {
            // Arrange
            var name = "Existing Brand";
            var description = "Existing Description";
            var command = new BrandCreateCommand()
            {
                Name = name,
                Description = description
            };

            _mockBrandRepository.Setup(r => r.GetByName(name)).ReturnsAsync(new Brand { Name = name });

            var expectedMessage = "Brand already exists.";
            _mockLogger.Setup(l => l.Handle(BrandCachedKeys.AlreadyExists, name, LogLevel.Warning))
                       .ReturnsAsync(expectedMessage);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.Conflict);
            result.Value.Should().Be(Guid.Empty);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails.Title.Should().Be(nameof(HttpStatusCode.Conflict));
            result.ErrorDetails.Message.Should().Be(expectedMessage);
            result.ErrorDetails.Errors.Should().BeNull();

            // Verify
            _mockBrandRepository.Verify(r => r.GetByName(name), Times.Once);
            _mockBrandRepository.Verify(r => r.Add(It.IsAny<Brand>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockLogger.Verify(l => l.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, It.IsAny<Exception>()), Times.Never);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.CreatedSuccessfully, LogLevel.Information), Times.Never);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.AlreadyExists, name, LogLevel.Warning), Times.Once);
        }

        [Fact]
        public async Task WhenExceptionIsThrown_ShouldReturnInternalServerError()
        {
            // Arrange
            var name = "Error Brand";
            var description = "Some Description";
            var command = new BrandCreateCommand()
            {
                Name = name,
                Description = description
            };

            var exceptionMessage = "DB failure";

            _mockBrandRepository.Setup(r => r.GetByName(name)).ThrowsAsync(new Exception(exceptionMessage));

            var expectedMessage = "Unexpected error.";
            _mockLogger
                .Setup(l => l.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, It.IsAny<Exception>()))
                .ReturnsAsync(expectedMessage);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Value.Should().Be(Guid.Empty);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails.Title.Should().Be(nameof(HttpStatusCode.InternalServerError));
            result.ErrorDetails.Message.Should().Be(expectedMessage);
            result.ErrorDetails.Errors.Should().BeNull();

            // Verify
            _mockBrandRepository.Verify(r => r.GetByName(name), Times.Once);
            _mockBrandRepository.Verify(r => r.Add(It.IsAny<Brand>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockLogger.Verify(l => l.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, It.IsAny<Exception>()), Times.Once);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.CreatedSuccessfully, LogLevel.Information), Times.Never);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.AlreadyExists, name, LogLevel.Warning), Times.Never);
        }
    }
}
