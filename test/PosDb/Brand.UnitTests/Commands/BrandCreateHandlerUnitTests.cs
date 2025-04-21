using Application.Behaviors;
using Application.Interfaces.UnitOfWorks;
using Application.UseCases.Brands.Commands.Create;
using Domain.Entities;
using Domain.Entities.Brands;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Brands.UnitTests.Commands
{
    public class BrandCreateHandlerUnitTests
    {
        private readonly Mock<IBrandRepository> _mockBrandRepository = new();
        private readonly Mock<ILogger<BrandCreateHandler>> _mockLogger = new();
        private readonly Mock<IPosDbUnitOfWork> _mockPosDbUnitOfWork = new();

        private readonly IMediator _mediator;

        public BrandCreateHandlerUnitTests()
        {
            _mockPosDbUnitOfWork.Setup(x => x.BrandRepository).Returns(_mockBrandRepository.Object);

            var services = new ServiceCollection();
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<BrandCreateCommand>();
            });

            services.AddScoped(_ => _mockPosDbUnitOfWork.Object);
            services.AddScoped(_ => _mockLogger.Object);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblyContaining<BrandCreateValidator>();

            var provider = services.BuildServiceProvider();
            _mediator = provider.GetRequiredService<IMediator>();
        }

        /* 
         * El Escenario
         * Lo que debe regresar
         */
        [Fact]
        public async Task WhenAllIsOk_ShouldReturnSuccess()
        {
            // Arrange
            var command = new BrandCreateCommand("Test Brand", "Test Description");

            _mockPosDbUnitOfWork
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.ErrorDetails.Should().BeNull();

            _mockBrandRepository.Verify(r => r.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task WhenNameIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var command = new BrandCreateCommand("", "Test Description");

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Name" &&
                e.ErrorMessage == "Name is required.");
            result.ErrorDetails.Status.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task WhenDescriptionIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var command = new BrandCreateCommand("Test Brand", "");

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails!.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Description" &&
                e.ErrorMessage == "Description is required.");
            result.ErrorDetails.Status.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task WhenNameAndDescriptionAreEmpty_ShouldReturnValidationErrors()
        {
            // Arrange
            var command = new BrandCreateCommand("", "");

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails!.Errors.Should().Contain(e =>
                e.PropertyName == "Name" &&
                e.ErrorMessage == "Name is required.");
            result.ErrorDetails!.Errors.Should().Contain(e =>
                e.PropertyName == "Description" &&
                e.ErrorMessage == "Description is required.");
            result.ErrorDetails.Status.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task WhenNameExceedsMaxLength_ShouldReturnValidationError()
        {
            // Arrange
            var longName = new string('A', BaseCatalog.MaxNameLength);
            var command = new BrandCreateCommand(longName, "Valid Description");

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails!.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Name" &&
                e.ErrorMessage == "Name must not exceed 64 characters.");
            result.ErrorDetails.Status.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task WhenDescriptionExceedsMaxLength_ShouldReturnValidationError()
        {
            // Arrange
            var longDescription = new string('B', (BaseCatalog.MaxDescriptionLength);
            var command = new BrandCreateCommand("Valid Name", longDescription);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails!.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Description" &&
                e.ErrorMessage == "Description must not exceed 256 characters.");
            result.ErrorDetails.Status.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task WhenBrandWithSameNameExists_ShouldReturnBadRequest()
        {
            // Arrange
            var command = new BrandCreateCommand("Test Brand", "Test Description");

            _mockBrandRepository
                .Setup(r => r.GetByName("Test Brand"))
                .ReturnsAsync(new Brand { Name = "Test Brand" });

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails!.Message.Should().Be("Brand with name Test Brand already exists.");
            result.ErrorDetails.Status.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task WhenExceptionIsThrown_ShouldReturnInternalServerError()
        {
            // Arrange
            var command = new BrandCreateCommand("Test Brand", "Test Description");

            _mockBrandRepository
                .Setup(r => r.GetByName(It.IsAny<string>()))
                .ReturnsAsync((Brand?)null);

            _mockBrandRepository
                .Setup(r => r.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()));

            _mockPosDbUnitOfWork
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails!.Message.Should().Be("Database error");
            result.ErrorDetails.Status.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
