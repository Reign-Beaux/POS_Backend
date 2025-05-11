using Application.Features.Brands;
using Application.Features.Brands.UseCases.Commands.Delete;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Azure.Core;
using Domain.Entities.Brands;

namespace BrandUnitTests.Commands
{
    public class BrandDeleteHandlerUnitTest
    {
        private readonly Mock<IPosDbUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<ILoggingMessagesService<BrandDeleteHandler>> _mockLogger = new();

        private readonly BrandDeleteHandler _handler;
        private readonly Mock<IBrandRepository> _mockBrandRepository = new();

        public BrandDeleteHandlerUnitTest()
        {
            _mockUnitOfWork.Setup(u => u.BrandRepository).Returns(_mockBrandRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _handler = new BrandDeleteHandler(_mockLogger.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task WhenAllIsOk_ShouldReturnUpdated()
        {
            // Arrange
            var brandId = Guid.NewGuid();
            var command = new BrandDeleteCommand(brandId);
            var brand = new Brand
            {
                Id = brandId,
                Name = "Test Brand",
                Description = "Test Description"
            };
            _mockBrandRepository.Setup(r => r.GetById(brandId)).ReturnsAsync(brand);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(HttpStatusCode.NoContent);
            result.Value.Should().Be(Unit.Value);
            result.ErrorDetails.Should().BeNull();

            // Verify
            _mockBrandRepository.Verify(r => r.GetById(command.Id), Times.Once);
            _mockBrandRepository.Verify(r => r.Delete(It.IsAny<Brand>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.NotFound, command.Id.ToString(), LogLevel.Warning), Times.Never);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.Deleted, It.IsAny<string>(), LogLevel.Information), Times.Once);
            _mockLogger.Verify(l => l.HandleExceptionMessage(BrandCachedKeys.ErrorDeleting, It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task WhenBrandNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var command = new BrandDeleteCommand(Guid.NewGuid());
            var expectedMessage = $"Brand not found.";
            _mockBrandRepository.Setup(r => r.GetById(command.Id)).ReturnsAsync((Brand?)null);

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
            _mockBrandRepository.Verify(r => r.GetById(command.Id), Times.Once);
            _mockBrandRepository.Verify(r => r.Delete(It.IsAny<Brand>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.NotFound, command.Id.ToString(), LogLevel.Warning), Times.Once);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.Deleted, It.IsAny<string>(), LogLevel.Information), Times.Never);
            _mockLogger.Verify(l => l.HandleExceptionMessage(BrandCachedKeys.ErrorDeleting, It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task WhenExceptionIsThrown_ShouldReturnInternalServerError()
        {
            // Arrange
            var command = new BrandDeleteCommand(Guid.NewGuid());
            var expectedMessage = "An error occurred while deleting the brand.";
            _mockBrandRepository.Setup(r => r.GetById(command.Id)).ThrowsAsync(new Exception(expectedMessage));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Value.Should().Be(Unit.Value);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails!.Title.Should().Be(nameof(HttpStatusCode.InternalServerError));
            result.ErrorDetails.Message.Should().Be(expectedMessage);
            result.ErrorDetails.Errors.Should().BeNull();

            // Verify
            _mockBrandRepository.Verify(r => r.GetById(command.Id), Times.Once);
            _mockBrandRepository.Verify(r => r.Delete(It.IsAny<Brand>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.NotFound, command.Id.ToString(), LogLevel.Warning), Times.Never);
            _mockLogger.Verify(l => l.Handle(BrandCachedKeys.Deleted, It.IsAny<string>(), LogLevel.Information), Times.Never);
            _mockLogger.Verify(l => l.HandleExceptionMessage(BrandCachedKeys.ErrorDeleting, It.IsAny<Exception>()), Times.Once);
        }
    }
}
