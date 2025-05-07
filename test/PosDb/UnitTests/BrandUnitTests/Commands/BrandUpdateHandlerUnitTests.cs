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
        }
    }
}
