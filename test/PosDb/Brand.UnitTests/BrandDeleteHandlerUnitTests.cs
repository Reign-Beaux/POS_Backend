using Application.Behaviors;
using Application.Interfaces.UnitOfWorks;
using Application.UseCases.Brands.Commands.Delete;
using Domain.Entities.Brands;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Brands.UnitTests
{
    public class BrandDeleteHandlerUnitTests
    {
        private readonly Mock<IPosDbUnitOfWork> _mockPosDbUnitOfWork = new();
        private readonly Mock<IBrandRepository> _mockBrandRepository = new();
        private readonly Mock<ILogger<BrandDeleteHandler>> _mockLogger = new();
        private readonly IMediator _mediator;

        public BrandDeleteHandlerUnitTests()
        {
            _mockPosDbUnitOfWork.Setup(x => x.BrandRepository).Returns(_mockBrandRepository.Object);

            var services = new ServiceCollection();

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<BrandDeleteCommand>();
            });

            services.AddScoped(_ => _mockPosDbUnitOfWork.Object);
            services.AddScoped(_ => _mockLogger.Object);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            var provider = services.BuildServiceProvider();
            _mediator = provider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task WhenBrandExists_ShouldReturnSuccess()
        {
            // Arrange
            var id = Guid.NewGuid();
            var brand = new Brand { Id = id, Name = "Test Brand", Description = "Test Desc" };
            var command = new BrandDeleteCommand(id);

            _mockBrandRepository.Setup(r => r.GetById(id)).ReturnsAsync(brand);
            _mockPosDbUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.ProblemDetails.Should().BeNull();
        }

        [Fact]
        public async Task WhenBrandDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new BrandDeleteCommand(id);

            _mockBrandRepository.Setup(r => r.GetById(id)).ReturnsAsync((Brand?)null);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ProblemDetails.Should().NotBeNull();
            result.ProblemDetails!.Status.Should().Be(HttpStatusCode.NotFound);
            result.ProblemDetails.Message.Should().Be("Brand not found.");
        }

        [Fact]
        public async Task WhenExceptionIsThrown_ShouldReturnInternalServerError()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new BrandDeleteCommand(id);

            _mockBrandRepository.Setup(r => r.GetById(id)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ProblemDetails.Should().NotBeNull();
            result.ProblemDetails!.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.ProblemDetails.Message.Should().Be("Unexpected error");
        }
    }
}
