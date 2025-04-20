using Application.Behaviors;
using Application.Interfaces.UnitOfWorks;
using Application.UseCases.Brands.Commands.Create;
using AutoMapper;
using Domain.Entities.Brands;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Brands.UnitTests
{
    public class BrandCreateHandlerUnitTests
    {
        private readonly Mock<IPosDbUnitOfWork> _mockPosDbUnitOfWork = new();
        private readonly Mock<IMapper> _mockMapper = new();
        private readonly Mock<IBrandRepository> _mockBrandRepository = new();
        private readonly IMediator _mediator;

        public BrandCreateHandlerUnitTests()
        {
            _mockPosDbUnitOfWork.Setup(x => x.BrandRepository).Returns(_mockBrandRepository.Object);

            // Configuración de servicios de MediatR con PipelineBehavior
            var services = new ServiceCollection();
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<BrandCreateCommand>();
            });

            services.AddScoped(_ => _mockPosDbUnitOfWork.Object);
            services.AddScoped(_ => _mockMapper.Object);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssemblyContaining<BrandCreateValidator>();

            var provider = services.BuildServiceProvider();
            _mediator = provider.GetRequiredService<IMediator>();
        }

        /* 
         * ¿Qué vamos a probar?
         * El Escenario
         * Lo que debe regresar
         */
        [Fact]
        public async Task BrandCreateHandler_WhenAllIsOk_ShouldReturnSuccess()
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
    }
}
