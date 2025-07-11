﻿using Application.Behaviors;
using Application.Features.Brands.DTOs;
using Application.Features.Brands.UseCases.Commands.Create;
using Application.OperationResults;
using BaseTests.Factories;
using Domain.Entities;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace BrandIntegrationTests.Commands
{
    public class BrandCreateIntegrationTests(POSWebApplicationFactory factory) : IClassFixture<POSWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();
        private const string RequestBaseUri = "/api/Brand";

        [Fact]
        public async Task WhenAllIsOk_ShouldReturnCreated()
        {
            // Arrange
            var command = new BrandCreateCommand()
            {
                Name = $"Test name {Guid.NewGuid()}",
                Description = "Test description"
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync(RequestBaseUri, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var location = response.Headers.Location;
            location.Should().NotBeNull();
            location!.AbsolutePath.Should().MatchRegex(@"^/api/Brand/[0-9a-fA-F\-]{36}$");

            var createdBrandId = await response.Content.ReadFromJsonAsync<Guid>();
            createdBrandId.Should().NotBeEmpty();

            // Additional Act
            var getResponse = await _client.GetAsync($"{RequestBaseUri}/{createdBrandId}");

            // Additional Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var brand = await getResponse.Content.ReadFromJsonAsync<BrandDTO>();
            brand.Should().NotBeNull();
            brand.Id.Should().Be(createdBrandId);
            brand.Name.Should().Be(command.Name);
            brand.Description.Should().Be(command.Description);
        }

        [Fact]
        public async Task WhenNameIsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            var command = new BrandCreateCommand()
            {
                Name = string.Empty,
                Description = "Test description"
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync(RequestBaseUri, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            result.Should().NotBeNull();
            result.Title.Should().Be(ValidationMessages.Title);
            result.Message.Should().Be(ValidationMessages.Message);
            result.Errors.Should().NotBeNull();
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == nameof(BrandCreateCommand.Name) &&
                e.ErrorMessage != string.Empty);
        }

        [Fact]
        public async Task WhenNameExceedsMaxLength_ShouldReturnValidationError()
        {
            // Arrange
            string NameTooLong = new('A', BaseCatalog.NameMaxLength + 1);
            var command = new BrandCreateCommand()
            {
                Name = NameTooLong,
                Description = "Test description"
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync(RequestBaseUri, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            result.Should().NotBeNull();
            result.Title.Should().Be(ValidationMessages.Title);
            result.Message.Should().Be(ValidationMessages.Message);
            result.Errors.Should().NotBeNull();
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == nameof(BrandCreateCommand.Name) &&
                e.ErrorMessage != string.Empty);
        }

        [Fact]
        public async Task WhenDescriptionIsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            var command = new BrandCreateCommand()
            {
                Name = "Test name",
                Description = string.Empty
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync(RequestBaseUri, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            result.Should().NotBeNull();
            result.Title.Should().Be(ValidationMessages.Title);
            result.Message.Should().Be(ValidationMessages.Message);
            result.Errors.Should().NotBeNull();
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == nameof(BrandCreateCommand.Description) &&
                e.ErrorMessage != string.Empty);
        }

        [Fact]
        public async Task WhenDescriptionExceedsMaxLength_ShouldReturnValidationError()
        {
            // Arrange
            string DescriptionTooLong = new('A', BaseCatalog.DescriptionMaxLength + 1);
            var command = new BrandCreateCommand()
            {
                Name = "Test name",
                Description = DescriptionTooLong
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync(RequestBaseUri, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            result.Should().NotBeNull();
            result.Title.Should().Be(ValidationMessages.Title);
            result.Message.Should().Be(ValidationMessages.Message);
            result.Errors.Should().NotBeNull();
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == nameof(BrandCreateCommand.Description) &&
                e.ErrorMessage != string.Empty);
        }

        [Fact]
        public async Task WhenAllFieldsAreEmpty_ShouldReturnValidationErrors()
        {
            // Arrange
            var command = new BrandCreateCommand()
            {
                Name = string.Empty,
                Description = string.Empty
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync(RequestBaseUri, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            result.Should().NotBeNull();
            result.Title.Should().Be(ValidationMessages.Title);
            result.Message.Should().Be(ValidationMessages.Message);
            result.Errors.Should().NotBeNull();
            result.Errors.Should().Contain(e =>
                e.PropertyName == nameof(BrandCreateCommand.Name) &&
                e.ErrorMessage != string.Empty);
            result.Errors.Should().Contain(e =>
                e.PropertyName == nameof(BrandCreateCommand.Description) &&
                e.ErrorMessage != string.Empty);
        }

        [Fact]
        public async Task WhenAllFieldsExceedMaxLength_ShouldReturnValidationErrors()
        {
            // Arrange
            string NameTooLong = new('A', BaseCatalog.NameMaxLength + 1);
            string DescriptionTooLong = new('A', BaseCatalog.DescriptionMaxLength + 1);
            var command = new BrandCreateCommand()
            {
                Name = NameTooLong,
                Description = DescriptionTooLong
            };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync(RequestBaseUri, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            result.Should().NotBeNull();
            result.Title.Should().Be(ValidationMessages.Title);
            result.Message.Should().Be(ValidationMessages.Message);
            result.Errors.Should().NotBeNull();
            result.Errors.Should().Contain(e =>
                e.PropertyName == nameof(BrandCreateCommand.Name) &&
                e.ErrorMessage != string.Empty);
            result.Errors.Should().Contain(e =>
                e.PropertyName == nameof(BrandCreateCommand.Description) &&
                e.ErrorMessage != string.Empty);
        }

        [Fact]
        public async Task WhenBrandAlreadyExists_ShouldReturnConflict()
        {
            // Arrange: Obtener todos los brands
            var getAllResponse = await _client.GetAsync(RequestBaseUri);
            getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var brands = await getAllResponse.Content.ReadFromJsonAsync<List<BrandDTO>>();
            brands.Should().NotBeNull();
            brands!.Count.Should().BeGreaterThan(0);

            var existingBrand = brands.First();

            var duplicateCommand = new BrandCreateCommand()
            {
                Name = existingBrand.Name,
                Description = existingBrand.Description
            };

            // Act: Intentar crear una marca con los mismos datos
            var postResponse = await _client.PostAsJsonAsync(RequestBaseUri, duplicateCommand);

            // Assert
            postResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);

            var error = await postResponse.Content.ReadFromJsonAsync<ErrorDetails>();
            error.Should().NotBeNull();
            error!.Title.Should().NotBeNullOrEmpty();
            error.Message.Should().NotBeNullOrEmpty();
            error.Errors.Should().BeNull();
        }
    }
}
