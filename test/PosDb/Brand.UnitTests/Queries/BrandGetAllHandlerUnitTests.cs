using Application.DTOs.Brands;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Application.UseCases.Brands.Queries.GetAll;
using Domain.Entities.Brands;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Brands.UnitTests.Queries
{
    public class BrandGetAllHandlerUnitTests
    {
        private readonly Mock<IBrandRepository> _mockBrandRepository = new();
        private readonly Mock<ILogger<BrandGetAllHandler>> _mockLogger = new();
        private readonly Mock<IMapper> _mockMapper = new();
        private readonly Mock<IPosDbUnitOfWork> _mockPosDbUnitOfWork = new();

        private readonly IMediator _mediator;

        public BrandGetAllHandlerUnitTests()
        {
            _mockPosDbUnitOfWork.Setup(x => x.BrandRepository).Returns(_mockBrandRepository.Object);
            var services = new ServiceCollection();
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<BrandGetAllQuery>();
            });

            services.AddScoped(_ => _mockPosDbUnitOfWork.Object);
            services.AddScoped(_ => _mockLogger.Object);
            services.AddScoped(_ => _mockMapper.Object);

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
            var command = new BrandGetAllQuery();
            var brands = new List<Brand>
            {
                new() { Id = Guid.NewGuid(), Name = "Brand 1", Description = "Description 1" },
                new() { Id = Guid.NewGuid(), Name = "Brand 2", Description = "Description 2" }
            };
            var brandDTOs = new List<BrandDTO>
            {
                new() { Id = Guid.NewGuid(), Name = "Brand 1", Description = "Description 1" },
                new() { Id = Guid.NewGuid(), Name = "Brand 2", Description = "Description 2" }
            };

            _mockBrandRepository.Setup(r => r.GetAll())
                    .ReturnsAsync(brands);

            _mockMapper.Setup(m => m.Map<IEnumerable<BrandDTO>>(It.IsAny<IEnumerable<Brand>>()))
                    .Returns(brandDTOs);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WhenExceptionIsThrown_ShouldReturnInternalServerError()
        {
            // Arrange
            var query = new BrandGetAllQuery();
            _mockBrandRepository.Setup(r => r.GetAll()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.ErrorDetails!.Message.Should().Contain("Database error");
        }
    }
}
