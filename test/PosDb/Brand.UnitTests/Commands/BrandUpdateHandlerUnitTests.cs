using Application.Behaviors;
using Application.Interfaces.UnitOfWorks;
using Application.UseCases.Brands.Commands.Update;
using Domain.Entities.Brands;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Brands.UnitTests.Commands
{
    public class BrandUpdateHandlerUnitTests
    {
        private readonly Mock<IBrandRepository> _mockBrandRepository = new();
        private readonly Mock<ILogger<BrandUpdateHandler>> _mockLogger = new();
        private readonly Mock<IMapper> _mockMapper = new();
        private readonly Mock<IPosDbUnitOfWork> _mockPosDbUnitOfWork = new();

        private readonly IMediator _mediator;

        public BrandUpdateHandlerUnitTests()
        {
            _mockPosDbUnitOfWork.Setup(x => x.BrandRepository).Returns(_mockBrandRepository.Object);

            var services = new ServiceCollection();
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<BrandUpdateHandler>();
            });

            services.AddScoped(_ => _mockPosDbUnitOfWork.Object);
            services.AddScoped(_ => _mockLogger.Object);
            services.AddScoped(_ => _mockMapper.Object);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblyContaining<BrandUpdateValidator>();

            var provider = services.BuildServiceProvider();
            _mediator = provider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task WhenBrandExists_ShouldReturnSuccess()
        {
            // Arrange
            var command = new BrandUpdateCommand(Guid.NewGuid(), "Updated Name", "Updated Description");
            var existingBrand = new Brand { Id = command.Id, Name = "Old", Description = "Old Desc" };

            _mockBrandRepository.Setup(r => r.GetById(command.Id))
                .ReturnsAsync(existingBrand);

            _mockBrandRepository.Setup(r => r.Update(It.IsAny<Brand>(), It.IsAny<CancellationToken>()));
            _mockPosDbUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WhenBrandDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var command = new BrandUpdateCommand(Guid.NewGuid(), "Name", "Description");

            _mockBrandRepository.Setup(r => r.GetById(command.Id)).ReturnsAsync((Brand?)null);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails.Status.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenNameIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var command = new BrandUpdateCommand(Guid.NewGuid(), "", "Valid Description");

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails!.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Name" && e.ErrorMessage == "Name is required.");
        }

        [Fact]
        public async Task WhenDescriptionIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var command = new BrandUpdateCommand(Guid.NewGuid(), "Valid Name", "");

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails!.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Description" && e.ErrorMessage == "Description is required.");
        }

        [Fact]
        public async Task WhenNameAndDescriptionAreEmpty_ShouldReturnValidationErrors()
        {
            // Arrange
            var command = new BrandUpdateCommand(Guid.NewGuid(), "", "");

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails!.Errors.Should().Contain(e =>
                e.PropertyName == "Name" && e.ErrorMessage == "Name is required.");
            result.ErrorDetails!.Errors.Should().Contain(e =>
                e.PropertyName == "Description" && e.ErrorMessage == "Description is required.");
        }

        [Fact]
        public async Task WhenNameExceedsMaxLength_ShouldReturnValidationError()
        {
            // Arrange
            var command = new BrandUpdateCommand(Guid.NewGuid(), new string('A', 65), "Valid Description");

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails!.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Name" && e.ErrorMessage == "Name must not exceed 64 characters.");
        }

        [Fact]
        public async Task WhenDescriptionExceedsMaxLength_ShouldReturnValidationError()
        {
            // Arrange
            var command = new BrandUpdateCommand(Guid.NewGuid(), "Valid Name", new string('B', 257));

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails!.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Description" && e.ErrorMessage == "Description must not exceed 256 characters.");
        }
    }
}
