using Application.Behaviors;
using Application.Features.Brands;
using Application.Features.Brands.UseCases.Commands.Create;
using Application.Interfaces.Caching;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWorks;
using Application.Shared.Catalogs;
using Domain.Entities;
using Domain.Entities.Brands;

namespace Brands.UnitTests.Commands
{
    static class MockFieldValues
    {
        public static string Description = "Test Description";
        public static string DescriptionTooLong = new('B', BaseCatalog.DescriptionMaxLength + 1);
        public static string Name = "Test Brand";
        public static string NameTooLong = new('A', BaseCatalog.NameMaxLength + 1);
    }

    static class ExpectedValidationMessages
    {
        public static string BrandAlreadyExists = $"Brand with name {MockFieldValues.Name} already exists.";
        public static string DatabaseError = "Database error.";
        public static string DescriptionIsRequired = "Description is required.";
        public static string DescriptionMustNotExceedMaxLength = $"Description must not exceed {BaseCatalog.DescriptionMaxLength} characters.";
        public static string NameIsRequired = "Name is required.";
        public static string NameMustNotExceedMaxLength = $"Name must not exceed {BaseCatalog.NameMaxLength} characters.";
    }

    public class BrandCreateHandlerUnitTests
    {
        private readonly Mock<ILocalizationCached> _mockLocalization = new();
        private readonly Mock<ILoggingMessagesService<BrandCreateHandler>> _mockLogginMessages = new();
        private readonly Mock<IPosDbUnitOfWork> _mockPosDbUnitOfWork = new();

        private readonly IMediator _mediator;

        public BrandCreateHandlerUnitTests()
        {
            Mock<IBrandRepository> mockBrandRepository = new();
            _mockPosDbUnitOfWork.Setup(x => x.BrandRepository).Returns(mockBrandRepository.Object);

            var services = new ServiceCollection();
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<BrandCreateHandler>();
            });

            services.AddScoped(_ => _mockLocalization.Object);
            services.AddScoped(_ => _mockLogginMessages.Object);
            services.AddScoped(_ => _mockPosDbUnitOfWork.Object);

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddValidatorsFromAssemblyContaining<BrandCreateHandler>();

            var provider = services.BuildServiceProvider();
            _mediator = provider.GetRequiredService<IMediator>();
        }

        ///* 
        // * El Escenario
        // * Lo que debe regresar
        // */

        [Fact]
        public async Task WhenAllIsOk_ShouldReturnSuccess()
        {
            // Arrange
            var command = new BrandCreateCommand(MockFieldValues.Name, MockFieldValues.Description);

            _mockPosDbUnitOfWork
                .Setup(r => r.BrandRepository.GetByName(MockFieldValues.Name))
                .ReturnsAsync((Brand?)null);
            _mockPosDbUnitOfWork
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.Value.Should().NotBeEmpty();
            result.Status.Should().Be(HttpStatusCode.Created);
            result.ErrorDetails.Should().BeNull();

            // Verify
            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Once);
            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.AlreadyExists, MockFieldValues.Name, LogLevel.Warning), Times.Never);
            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.CreatedSuccessfully, LogLevel.Information), Times.Once);
            _mockLogginMessages.Verify(u => u.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, MockFieldValues.Name, It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task WhenNameIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var command = new BrandCreateCommand(string.Empty, MockFieldValues.Description);
            string expectedErrorMessage = ExpectedValidationMessages.NameIsRequired;

            _mockLocalization
                .Setup(l => l.GetText(CatalogCachedKeys.NameIsRequired))
                .ReturnsAsync(expectedErrorMessage);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.Value.Should().BeEmpty();
            result.Status.Should().Be(HttpStatusCode.BadRequest);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails.Title.Should().Be(ValidationMessages.Title);
            result.ErrorDetails.Message.Should().Be(ValidationMessages.Message);
            result.ErrorDetails.Errors.Should().ContainSingle(e =>
                e.PropertyName == nameof(BrandCreateCommand.Name) &&
                e.ErrorMessage == expectedErrorMessage);

            // Verify
            _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.NameIsRequired), Times.Once);

            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Never);
            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.AlreadyExists, MockFieldValues.Name, LogLevel.Warning), Times.Never);
            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.CreatedSuccessfully, LogLevel.Information), Times.Never);
            _mockLogginMessages.Verify(u => u.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, MockFieldValues.Name, It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task WhenNameExceedsMaxLength_ShouldReturnValidationError()
        {
            // Arrange
            var command = new BrandCreateCommand(MockFieldValues.NameTooLong, MockFieldValues.Description);

            string expectedErrorMessage = ExpectedValidationMessages.NameMustNotExceedMaxLength;

            _mockLocalization
                .Setup(l => l.GetText(CatalogCachedKeys.NameMaxLength))
                .ReturnsAsync(expectedErrorMessage);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.Value.Should().BeEmpty();
            result.Status.Should().Be(HttpStatusCode.BadRequest);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails.Title.Should().Be(ValidationMessages.Title);
            result.ErrorDetails.Message.Should().Be(ValidationMessages.Message);
            result.ErrorDetails.Errors.Should().ContainSingle(e =>
                e.PropertyName == nameof(BrandCreateCommand.Name) &&
                e.ErrorMessage == expectedErrorMessage);

            // Verify
            _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.NameMaxLength), Times.Once);

            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Never);
            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.AlreadyExists, MockFieldValues.Name, LogLevel.Warning), Times.Never);
            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.CreatedSuccessfully, LogLevel.Information), Times.Never);
            _mockLogginMessages.Verify(u => u.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, MockFieldValues.Name, It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task WhenDescriptionIsEmpty_ShouldReturnValidationError()
        {
            // Arrange
            var command = new BrandCreateCommand(MockFieldValues.Name, string.Empty);
            string expectedErrorMessage = ExpectedValidationMessages.DescriptionIsRequired;

            _mockLocalization
                .Setup(l => l.GetText(CatalogCachedKeys.DescriptionIsRequired))
                .ReturnsAsync(expectedErrorMessage);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.Value.Should().BeEmpty();
            result.Status.Should().Be(HttpStatusCode.BadRequest);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails.Title.Should().Be(ValidationMessages.Title);
            result.ErrorDetails.Message.Should().Be(ValidationMessages.Message);
            result.ErrorDetails.Errors.Should().ContainSingle(e =>
                e.PropertyName == nameof(BrandCreateCommand.Description) &&
                e.ErrorMessage == expectedErrorMessage);

            // Verify
            _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.DescriptionIsRequired), Times.Once);

            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Never);
            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.AlreadyExists, MockFieldValues.Name, LogLevel.Warning), Times.Never);
            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.CreatedSuccessfully, LogLevel.Information), Times.Never);
            _mockLogginMessages.Verify(u => u.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, MockFieldValues.Name, It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task WhenDescriptionExceedsMaxLength_ShouldReturnValidationError()
        {
            // Arrange
            var command = new BrandCreateCommand(MockFieldValues.Name, MockFieldValues.DescriptionTooLong);
            string expectedErrorMessage = ExpectedValidationMessages.DescriptionMustNotExceedMaxLength;

            _mockLocalization
                .Setup(l => l.GetText(CatalogCachedKeys.DescriptionMaxLength))
                .ReturnsAsync(expectedErrorMessage);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.Value.Should().BeEmpty();
            result.Status.Should().Be(HttpStatusCode.BadRequest);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails.Title.Should().Be(ValidationMessages.Title);
            result.ErrorDetails.Message.Should().Be(ValidationMessages.Message);
            result.ErrorDetails.Errors.Should().ContainSingle(e =>
                e.PropertyName == nameof(BrandCreateCommand.Description) &&
                e.ErrorMessage == expectedErrorMessage);

            // Verify
            _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.DescriptionMaxLength), Times.Once);

            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Never);
            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.AlreadyExists, MockFieldValues.Name, LogLevel.Warning), Times.Never);
            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.CreatedSuccessfully, LogLevel.Information), Times.Never);
            _mockLogginMessages.Verify(u => u.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, MockFieldValues.Name, It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task WhenAllFieldsAreEmpty_ShouldReturnValidationErrors()
        {
            // Arrange
            var command = new BrandCreateCommand(string.Empty, string.Empty);
            string expectedNameErrorMessage = ExpectedValidationMessages.NameIsRequired;
            string expectedDescriptionErrorMessage = ExpectedValidationMessages.DescriptionIsRequired;

            _mockLocalization
                .Setup(l => l.GetText(CatalogCachedKeys.NameIsRequired))
                .ReturnsAsync(expectedNameErrorMessage);
            _mockLocalization
                .Setup(l => l.GetText(CatalogCachedKeys.DescriptionIsRequired))
                .ReturnsAsync(expectedDescriptionErrorMessage);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.Value.Should().BeEmpty();
            result.Status.Should().Be(HttpStatusCode.BadRequest);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails.Title.Should().Be(ValidationMessages.Title);
            result.ErrorDetails.Message.Should().Be(ValidationMessages.Message);
            result.ErrorDetails.Errors.Should().Contain(e =>
                e.PropertyName == nameof(BrandCreateCommand.Name) &&
                e.ErrorMessage == expectedNameErrorMessage);
            result.ErrorDetails.Errors.Should().Contain(e =>
                e.PropertyName == nameof(BrandCreateCommand.Description) &&
                e.ErrorMessage == expectedDescriptionErrorMessage);

            // Verify
            _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.NameIsRequired), Times.Once);
            _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.DescriptionIsRequired), Times.Once);

            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Never);
            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.AlreadyExists, MockFieldValues.Name, LogLevel.Warning), Times.Never);
            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.CreatedSuccessfully, LogLevel.Information), Times.Never);
            _mockLogginMessages.Verify(u => u.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, MockFieldValues.Name, It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task WhenAllFieldsExceedMaxLength_ShouldReturnValidationErrors()
        {
            // Arrange
            var command = new BrandCreateCommand(MockFieldValues.NameTooLong, MockFieldValues.DescriptionTooLong);
            string expectedNameErrorMessage = ExpectedValidationMessages.NameMustNotExceedMaxLength;
            string expectedDescriptionErrorMessage = ExpectedValidationMessages.DescriptionMustNotExceedMaxLength;

            _mockLocalization
                .Setup(l => l.GetText(CatalogCachedKeys.NameMaxLength))
                .ReturnsAsync(expectedNameErrorMessage);
            _mockLocalization
                .Setup(l => l.GetText(CatalogCachedKeys.DescriptionMaxLength))
                .ReturnsAsync(expectedDescriptionErrorMessage);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.Value.Should().BeEmpty();
            result.Status.Should().Be(HttpStatusCode.BadRequest);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails.Title.Should().Be(ValidationMessages.Title);
            result.ErrorDetails.Message.Should().Be(ValidationMessages.Message);
            result.ErrorDetails.Errors.Should().Contain(e =>
                e.PropertyName == nameof(BrandCreateCommand.Name) &&
                e.ErrorMessage == expectedNameErrorMessage);
            result.ErrorDetails!.Errors.Should().Contain(e =>
                e.PropertyName == nameof(BrandCreateCommand.Description) &&
                e.ErrorMessage == expectedDescriptionErrorMessage);

            // Verify
            _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.NameMaxLength), Times.Once);
            _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.DescriptionMaxLength), Times.Once);

            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Never);
            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.AlreadyExists, MockFieldValues.Name, LogLevel.Warning), Times.Never);
            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.CreatedSuccessfully, LogLevel.Information), Times.Never);
            _mockLogginMessages.Verify(u => u.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, MockFieldValues.Name, It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task WhenBrandAlreadyExists_ShouldReturnConflict()
        {
            // Arrange
            var command = new BrandCreateCommand(MockFieldValues.Name, MockFieldValues.Description);

            _mockPosDbUnitOfWork
                .Setup(r => r.BrandRepository.GetByName(MockFieldValues.Name))
                .ReturnsAsync(new Brand { Name = MockFieldValues.Name });
            _mockPosDbUnitOfWork
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _mockLogginMessages
                .Setup(r => r.Handle(BrandCachedKeys.AlreadyExists, MockFieldValues.Name, LogLevel.Warning))
                .ReturnsAsync(ExpectedValidationMessages.BrandAlreadyExists);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.Value.Should().BeEmpty();
            result.Status.Should().Be(HttpStatusCode.Conflict);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails.Title.Should().Be(nameof(HttpStatusCode.Conflict));
            result.ErrorDetails.Message.Should().Be(ExpectedValidationMessages.BrandAlreadyExists);
            result.ErrorDetails.Errors.Should().BeNull();

            // Verify
            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(MockFieldValues.Name), Times.Once);
            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.AlreadyExists, MockFieldValues.Name, LogLevel.Warning), Times.Once);
            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.CreatedSuccessfully, LogLevel.Information), Times.Never);
            _mockLogginMessages.Verify(u => u.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, MockFieldValues.Name, It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task WhenExceptionIsThrown_ShouldReturnInternalServerError()
        {
            // Arrange
            var command = new BrandCreateCommand(MockFieldValues.Name, MockFieldValues.Description);

            _mockPosDbUnitOfWork
                .Setup(r => r.BrandRepository.GetByName(MockFieldValues.Name))
                .ReturnsAsync((Brand?)null);
            _mockPosDbUnitOfWork
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(ExpectedValidationMessages.DatabaseError));
            _mockLocalization
                .Setup(l => l.GetText(BrandCachedKeys.ErrorCreating))
                .ReturnsAsync(ExpectedValidationMessages.DatabaseError);
            _mockLogginMessages
                .Setup(r => r.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, MockFieldValues.Name, It.IsAny<Exception>()))
                .ReturnsAsync(ExpectedValidationMessages.DatabaseError);

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.Value.Should().BeEmpty();
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.ErrorDetails.Should().NotBeNull();
            result.ErrorDetails.Title.Should().Be(nameof(HttpStatusCode.InternalServerError));
            result.ErrorDetails.Message.Should().Be(ExpectedValidationMessages.DatabaseError);
            result.ErrorDetails.Errors.Should().BeNull();

            // Verify
            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(MockFieldValues.Name), Times.Once);
            _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.AlreadyExists, MockFieldValues.Name, LogLevel.Warning), Times.Never);
            _mockLogginMessages.Verify(u => u.Handle(BrandCachedKeys.CreatedSuccessfully, LogLevel.Information), Times.Never);
            _mockLogginMessages.Verify(u => u.HandleExceptionMessage(BrandCachedKeys.ErrorCreating, MockFieldValues.Name, It.IsAny<Exception>()), Times.Once);
        }
    }
}
