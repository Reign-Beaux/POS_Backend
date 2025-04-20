using Application.Behaviors;
using Application.Interfaces.UnitOfWorks;
using Application.UseCases.Brands.Commands.Create;
using Domain.Entities.Brands;
using Microsoft.Extensions.Logging;

namespace Brands.UnitTests
{
    public class BrandCreateHandlerUnitTests
    {
        private readonly Mock<IBrandRepository> _mockBrandRepository = new();
        private readonly Mock<ILogger<BrandCreateHandler>> _mockLogger = new();
        private readonly Mock<IMapper> _mockMapper = new();
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
            services.AddScoped(_ => _mockMapper.Object);
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

            _mockMapper.Setup(m => m.Map<Brand>(It.IsAny<BrandCreateCommand>()))
                .Returns(new Brand { Name = command.Name, Description = command.Description });

            _mockBrandRepository.Setup(r => r.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()));
            _mockPosDbUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);
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
            result.ProblemDetails.Should().NotBeNull();
            result.ProblemDetails!.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Name" &&
                e.ErrorMessage == "Name is required.");
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
            result.ProblemDetails.Should().NotBeNull();
            result.ProblemDetails!.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Description" &&
                e.ErrorMessage == "Description is required.");
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
            result.ProblemDetails.Should().NotBeNull();
            result.ProblemDetails!.Errors.Should().Contain(e =>
                e.PropertyName == "Name" &&
                e.ErrorMessage == "Name is required.");

            result.ProblemDetails!.Errors.Should().Contain(e =>
                e.PropertyName == "Description" &&
                e.ErrorMessage == "Description is required.");
        }

        [Fact]
        public async Task WhenNameExceedsMaxLength_ShouldReturnValidationError()
        {
            // Arrange
            var longName = new string('A', 65);
            var command = new BrandCreateCommand(longName, "Valid Description");

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ProblemDetails.Should().NotBeNull();
            result.ProblemDetails!.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Name" &&
                e.ErrorMessage == "Name must not exceed 64 characters.");
        }

        [Fact]
        public async Task WhenDescriptionExceedsMaxLength_ShouldReturnValidationError()
        {
            // Arrange
            var longDescription = new string('B', 257);
            var command = new BrandCreateCommand("Valid Name", longDescription);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ProblemDetails.Should().NotBeNull();
            result.ProblemDetails!.Errors.Should().ContainSingle(e =>
                e.PropertyName == "Description" &&
                e.ErrorMessage == "Description must not exceed 256 characters.");
        }
    }
}
