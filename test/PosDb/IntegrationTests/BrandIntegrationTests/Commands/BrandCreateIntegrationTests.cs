using Application.Behaviors;
using Application.Features.Brands.DTOs;
using Application.Features.Brands.UseCases.Commands.Create;
using Application.OperationResults;
using BaseTests.Factories;
using Enyim.Caching.Memcached.Protocol.Text;
using FluentAssertions;
using MediatR;
using System.Net;
using System.Net.Http.Json;

namespace BrandIntegrationTests.Commands
{
    public class BrandCreateIntegrationTests(POSWebApplicationFactory factory) : IClassFixture<POSWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task WhenAllIsOk_ShouldReturnCreated()
        {
            // Arrange
            var command = new BrandCreateCommand($"Test name {Guid.NewGuid()}", "Descripción de prueba");
            const string requestUri = "/api/Brand";

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync(requestUri, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var location = response.Headers.Location;
            location.Should().NotBeNull();
            location!.AbsolutePath.Should().MatchRegex(@"^/api/Brand/[0-9a-fA-F\-]{36}$");

            var createdBrandId = await response.Content.ReadFromJsonAsync<Guid>();
            createdBrandId.Should().NotBeEmpty();

            // Additional Act
            var getResponse = await _client.GetAsync($"{requestUri}/{createdBrandId}");

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
            var command = new BrandCreateCommand(string.Empty, "Descripción de prueba");
            const string requestUri = "/api/Brand";
            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync(requestUri, command);
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
    }
}
