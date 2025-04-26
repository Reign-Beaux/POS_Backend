using Application.Behaviors;
using Application.Interfaces.Caching;
using Application.Interfaces.UnitOfWorks;
using Application.Features.Brands.Commands.Create;
using Domain.Entities;
using Domain.Entities.Brands;
using Microsoft.Extensions.Logging;
using System.Net;
using Application.Shared.Catalogs;

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
        //private readonly Mock<ILocalizationCached> _mockLocalization = new();
        //private readonly Mock<ILogger<BrandCreateHandler>> _mockLogger = new();
        //private readonly Mock<IPosDbUnitOfWork> _mockPosDbUnitOfWork = new();

        //private readonly IMediator _mediator;

        //public BrandCreateHandlerUnitTests()
        //{
        //    Mock<IBrandRepository> mockBrandRepository = new();
        //    _mockPosDbUnitOfWork.Setup(x => x.BrandRepository).Returns(mockBrandRepository.Object);

        //    var services = new ServiceCollection();
        //    services.AddMediatR(config =>
        //    {
        //        config.RegisterServicesFromAssemblyContaining<BrandCreateHandler>();
        //    });

        //    services.AddScoped(_ => _mockLocalization.Object);
        //    services.AddScoped(_ => _mockPosDbUnitOfWork.Object);
        //    services.AddScoped(_ => _mockLogger.Object);

        //    services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        //    services.AddValidatorsFromAssemblyContaining<BrandCreateHandler>();

        //    var provider = services.BuildServiceProvider();
        //    _mediator = provider.GetRequiredService<IMediator>();
        //}

        ///* 
        // * El Escenario
        // * Lo que debe regresar
        // */

        //[Fact]
        //public async Task WhenAllIsOk_ShouldReturnSuccess()
        //{
        //    // Arrange
        //    var command = new BrandCreateCommand(MockFieldValues.Name, MockFieldValues.Description);

        //    _mockPosDbUnitOfWork
        //        .Setup(r => r.BrandRepository.GetByName(MockFieldValues.Name))
        //        .ReturnsAsync((Brand?)null);
        //    _mockPosDbUnitOfWork
        //        .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(1);

        //    // Act
        //    var result = await _mediator.Send(command);

        //    // Assert
        //    result.IsSuccess.Should().BeTrue();
        //    result.ErrorDetails.Should().BeNull();

        //    // Verify
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository, Times.Exactly(2));
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Once);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Once);
        //    _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        //}

        //[Fact]
        //public async Task WhenNameIsEmpty_ShouldReturnValidationError()
        //{
        //    // Arrange
        //    var command = new BrandCreateCommand(string.Empty, MockFieldValues.Description);
        //    string expectedErrorMessage = ExpectedValidationMessages.NameIsRequired;

        //    _mockLocalization
        //        .Setup(l => l.GetText(CatalogCachedKeys.NameIsRequired))
        //        .ReturnsAsync(expectedErrorMessage);

        //    // Act
        //    var result = await _mediator.Send(command);

        //    // Assert
        //    result.IsSuccess.Should().BeFalse();
        //    result.ErrorDetails.Should().NotBeNull();
        //    result.ErrorDetails.Errors.Should().ContainSingle(e =>
        //        e.PropertyName == nameof(BrandCreateCommand.Name) &&
        //        e.ErrorMessage == expectedErrorMessage);
        //    result.ErrorDetails.Status.Should().Be(HttpStatusCode.BadRequest);

        //    // Verify
        //    _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.NameIsRequired), Times.Once);

        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository, Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        //}

        //[Fact]
        //public async Task WhenNameExceedsMaxLength_ShouldReturnValidationError()
        //{
        //    // Arrange
        //    var command = new BrandCreateCommand(MockFieldValues.NameTooLong, MockFieldValues.Description);

        //    string expectedErrorMessage = ExpectedValidationMessages.NameMustNotExceedMaxLength;

        //    _mockLocalization
        //        .Setup(l => l.GetText(CatalogCachedKeys.NameMaxLength))
        //        .ReturnsAsync(expectedErrorMessage);

        //    // Act
        //    var result = await _mediator.Send(command);

        //    // Assert
        //    result.IsSuccess.Should().BeFalse();
        //    result.ErrorDetails.Should().NotBeNull();
        //    result.ErrorDetails!.Errors.Should().ContainSingle(e =>
        //        e.PropertyName == nameof(BrandCreateCommand.Name) &&
        //        e.ErrorMessage == expectedErrorMessage);
        //    result.ErrorDetails.Status.Should().Be(HttpStatusCode.BadRequest);

        //    // Verify
        //    _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.NameMaxLength), Times.Once);

        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository, Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        //}

        //[Fact]
        //public async Task WhenDescriptionIsEmpty_ShouldReturnValidationError()
        //{
        //    // Arrange
        //    var command = new BrandCreateCommand(MockFieldValues.Name, string.Empty);
        //    string expectedErrorMessage = ExpectedValidationMessages.DescriptionIsRequired;

        //    _mockLocalization
        //        .Setup(l => l.GetText(CatalogCachedKeys.DescriptionIsRequired))
        //        .ReturnsAsync(expectedErrorMessage);

        //    // Act
        //    var result = await _mediator.Send(command);

        //    // Assert
        //    result.IsSuccess.Should().BeFalse();
        //    result.ErrorDetails.Should().NotBeNull();
        //    result.ErrorDetails!.Errors.Should().ContainSingle(e =>
        //        e.PropertyName == nameof(BrandCreateCommand.Description) &&
        //        e.ErrorMessage == expectedErrorMessage);
        //    result.ErrorDetails.Status.Should().Be(HttpStatusCode.BadRequest);

        //    // Verify
        //    _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.DescriptionIsRequired), Times.Once);

        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository, Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        //}

        //[Fact]
        //public async Task WhenDescriptionExceedsMaxLength_ShouldReturnValidationError()
        //{
        //    // Arrange
        //    var command = new BrandCreateCommand(MockFieldValues.Name, MockFieldValues.DescriptionTooLong);
        //    string expectedErrorMessage = ExpectedValidationMessages.DescriptionMustNotExceedMaxLength;

        //    _mockLocalization
        //        .Setup(l => l.GetText(CatalogCachedKeys.DescriptionMaxLength))
        //        .ReturnsAsync(expectedErrorMessage);

        //    // Act
        //    var result = await _mediator.Send(command);

        //    // Assert
        //    result.IsSuccess.Should().BeFalse();
        //    result.ErrorDetails.Should().NotBeNull();
        //    result.ErrorDetails!.Errors.Should().ContainSingle(e =>
        //        e.PropertyName == nameof(BrandCreateCommand.Description) &&
        //        e.ErrorMessage == expectedErrorMessage);
        //    result.ErrorDetails.Status.Should().Be(HttpStatusCode.BadRequest);

        //    // Verify
        //    _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.DescriptionMaxLength), Times.Once);

        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository, Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        //}

        //[Fact]
        //public async Task WhenAllFieldsAreEmpty_ShouldReturnValidationErrors()
        //{
        //    // Arrange
        //    var command = new BrandCreateCommand(string.Empty, string.Empty);
        //    string expectedNameErrorMessage = ExpectedValidationMessages.NameIsRequired;
        //    string expectedDescriptionErrorMessage = ExpectedValidationMessages.DescriptionIsRequired;

        //    _mockLocalization
        //        .Setup(l => l.GetText(CatalogCachedKeys.NameIsRequired))
        //        .ReturnsAsync(expectedNameErrorMessage);
        //    _mockLocalization
        //        .Setup(l => l.GetText(CatalogCachedKeys.DescriptionIsRequired))
        //        .ReturnsAsync(expectedDescriptionErrorMessage);

        //    // Act
        //    var result = await _mediator.Send(command);

        //    // Assert
        //    result.IsSuccess.Should().BeFalse();
        //    result.ErrorDetails.Should().NotBeNull();
        //    result.ErrorDetails!.Errors.Should().Contain(e =>
        //        e.PropertyName == nameof(BrandCreateCommand.Name) &&
        //        e.ErrorMessage == expectedNameErrorMessage);
        //    result.ErrorDetails!.Errors.Should().Contain(e =>
        //        e.PropertyName == nameof(BrandCreateCommand.Description) &&
        //        e.ErrorMessage == expectedDescriptionErrorMessage);
        //    result.ErrorDetails.Status.Should().Be(HttpStatusCode.BadRequest);

        //    // Verify
        //    _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.NameIsRequired), Times.Once);
        //    _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.DescriptionIsRequired), Times.Once);

        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository, Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        //}

        //[Fact]
        //public async Task WhenAllFieldsExceedMaxLength_ShouldReturnValidationErrors()
        //{
        //    // Arrange
        //    var command = new BrandCreateCommand(MockFieldValues.NameTooLong, MockFieldValues.DescriptionTooLong);
        //    string expectedNameErrorMessage = ExpectedValidationMessages.NameMustNotExceedMaxLength;
        //    string expectedDescriptionErrorMessage = ExpectedValidationMessages.DescriptionMustNotExceedMaxLength;

        //    _mockLocalization
        //        .Setup(l => l.GetText(CatalogCachedKeys.NameMaxLength))
        //        .ReturnsAsync(expectedNameErrorMessage);
        //    _mockLocalization
        //        .Setup(l => l.GetText(CatalogCachedKeys.DescriptionMaxLength))
        //        .ReturnsAsync(expectedDescriptionErrorMessage);

        //    // Act
        //    var result = await _mediator.Send(command);

        //    // Assert
        //    result.IsSuccess.Should().BeFalse();
        //    result.ErrorDetails.Should().NotBeNull();
        //    result.ErrorDetails!.Errors.Should().Contain(e =>
        //        e.PropertyName == nameof(BrandCreateCommand.Name) &&
        //        e.ErrorMessage == expectedNameErrorMessage);
        //    result.ErrorDetails!.Errors.Should().Contain(e =>
        //        e.PropertyName == nameof(BrandCreateCommand.Description) &&
        //        e.ErrorMessage == expectedDescriptionErrorMessage);
        //    result.ErrorDetails.Status.Should().Be(HttpStatusCode.BadRequest);

        //    // Verify
        //    _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.NameMaxLength), Times.Once);
        //    _mockLocalization.Verify(l => l.GetText(CatalogCachedKeys.DescriptionMaxLength), Times.Once);

        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository, Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(It.IsAny<string>()), Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        //}

        //[Fact]
        //public async Task WhenBrandAlreadyExists_ShouldReturnBadRequest()
        //{
        //    // Arrange
        //    var command = new BrandCreateCommand(MockFieldValues.Name, MockFieldValues.Description);

        //    _mockPosDbUnitOfWork
        //        .Setup(r => r.BrandRepository.GetByName(MockFieldValues.Name))
        //        .ReturnsAsync(new Brand { Name = MockFieldValues.Name });
        //    _mockPosDbUnitOfWork
        //        .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(1);

        //    // Act
        //    var result = await _mediator.Send(command);

        //    // Assert
        //    result.IsSuccess.Should().BeFalse();
        //    result.ErrorDetails.Should().NotBeNull();
        //    result.ErrorDetails!.Message.Should().Be(ExpectedValidationMessages.BrandAlreadyExists);
        //    result.ErrorDetails.Status.Should().Be(HttpStatusCode.BadRequest);

        //    // Verify
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository, Times.Once);

        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.GetByName(MockFieldValues.Name), Times.Once);
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Never);
        //    _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        //    _mockLogger.Verify(x => x.LogWarning(), Times.Once);
        //}

        //[Fact]
        //public async Task WhenExceptionIsThrown_ShouldReturnInternalServerError()
        //{
        //    // Arrange
        //    var command = new BrandCreateCommand(MockFieldValues.Name, MockFieldValues.Description);

        //    _mockBrandRepository
        //        .Setup(r => r.GetByName(It.IsAny<string>()))
        //        .ReturnsAsync((Brand?)null);

        //    _mockPosDbUnitOfWork
        //        .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
        //        .ThrowsAsync(new Exception(ExpectedValidationMessages.DatabaseError));

        //    // Act
        //    var result = await _mediator.Send(command);

        //    // Assert
        //    result.IsSuccess.Should().BeFalse();
        //    result.ErrorDetails.Should().NotBeNull();
        //    result.ErrorDetails!.Message.Should().Be(ExpectedValidationMessages.DatabaseError);
        //    result.ErrorDetails.Status.Should().Be(HttpStatusCode.InternalServerError);

        //    // Verify
        //    _mockPosDbUnitOfWork.Verify(u => u.BrandRepository, Times.Once);
        //    _mockBrandRepository.Verify(r => r.GetByName(It.IsAny<string>()), Times.Once);
        //    _mockBrandRepository.Verify(r => r.Add(It.IsAny<Brand>(), It.IsAny<CancellationToken>()), Times.Once);
        //    _mockPosDbUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        //    _mockLogger.Verify(x => x.Log(
        //        LogLevel.Error,
        //        It.IsAny<EventId>(),
        //        It.IsAny<It.IsAnyType>(),
        //        It.IsAny<Exception>(),
        //        It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        //}
    }
}
