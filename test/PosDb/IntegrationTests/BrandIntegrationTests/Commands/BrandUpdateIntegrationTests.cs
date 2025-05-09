using Application.Features.Brands.DTOs;
using Application.Features.Brands.UseCases.Commands.Create;
using Application.Features.Brands.UseCases.Commands.Update;
using Application.OperationResults;
using BaseTests.Factories;
using Domain.Entities;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace BrandIntegrationTests.Commands
{
    public class BrandUpdateIntegrationTests(POSWebApplicationFactory factory) : IClassFixture<POSWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();
        private const string RequestBaseUri = "/api/Brand";

        private async Task<BrandDTO?> CreateTestBrandAsync(string name, string? description)
        {
            var createCommand = new BrandCreateCommand { Name = name, Description = description };
            var postResponse = await _client.PostAsJsonAsync(RequestBaseUri, createCommand);

            if (postResponse.StatusCode != HttpStatusCode.Created)
                return null;

            var createdBrandId = await postResponse.Content.ReadFromJsonAsync<Guid>();
            if (createdBrandId == Guid.Empty)
                return null;

            var getResponse = await _client.GetAsync($"{RequestBaseUri}/{createdBrandId}");
            if (getResponse.StatusCode != HttpStatusCode.OK)
                return null;

            var brandDto = await getResponse.Content.ReadFromJsonAsync<BrandDTO>();
            if (brandDto == null || brandDto.Id == null || brandDto.Id.Value != createdBrandId)
                return null;

            return brandDto;
        }

        [Fact]
        public async Task WhenAllIsOk_ShouldReturnNoContentAndUpdateBrand()
        {
            // Arrange: Crear una marca para actualizar
            var initialBrand = await CreateTestBrandAsync($"TestBrand Ok {Guid.NewGuid()}", "Initial Ok Description");
            initialBrand.Should().NotBeNull("Debería poder crear una marca para la prueba de actualización.");
            initialBrand!.Id.Should().NotBeNull();

            var brandIdToUpdate = initialBrand.Id.Value;

            var updateCommand = new BrandUpdateCommand
            {
                Id = brandIdToUpdate, // ID está en el cuerpo del comando
                Name = $"Updated Ok Brand Name {Guid.NewGuid()}",
                Description = "Updated Ok Brand Description"
            };
            // La ruta para PUT no incluye el ID, es solo /api/Brand
            string requestUriForPut = RequestBaseUri;

            // Act: Actualizar la marca
            HttpResponseMessage updateResponse = await _client.PutAsJsonAsync(requestUriForPut, updateCommand);

            // Assert: Verificar la respuesta de actualización
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, "La actualización exitosa debería devolver 204 No Content.");

            // Additional Act & Assert: Obtener la marca para verificar que se actualizó correctamente
            var getResponse = await _client.GetAsync($"{RequestBaseUri}/{brandIdToUpdate}"); // GET usa el ID en la ruta
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedBrand = await getResponse.Content.ReadFromJsonAsync<BrandDTO>();
            updatedBrand.Should().NotBeNull();
            updatedBrand!.Id.Should().NotBeNull();
            updatedBrand.Id.Value.Should().Be(brandIdToUpdate);
            updatedBrand.Name.Should().Be(updateCommand.Name);
            updatedBrand.Description.Should().Be(updateCommand.Description);
        }

        [Fact]
        public async Task WhenBrandToUpdateNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistentBrandId = Guid.NewGuid();
            var updateCommand = new BrandUpdateCommand
            {
                Id = nonExistentBrandId, // ID (no existente) en el cuerpo
                Name = "Try Update Non Existent",
                Description = "This should fail"
            };
            string requestUriForPut = RequestBaseUri;

            // Act
            HttpResponseMessage response = await _client.PutAsJsonAsync(requestUriForPut, updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            // Puedes verificar el cuerpo del error si tu `HandleErrorResponse` devuelve un `ErrorDetails` específico
            // var error = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            // error.Should().NotBeNull();
            // error.Title.Should().Contain("Not Found"); // Ajusta según tu implementación
        }

        [Fact]
        public async Task WhenIdIsNullInUpdateCommand_ShouldReturnBadRequest()
        {
            // Arrange
            // No necesitamos una marca existente para este test si la validación del ID es lo primero.
            var updateCommand = new BrandUpdateCommand
            {
                Id = null, // ID inválido (nulo)
                Name = "Valid Name",
                Description = "Valid Description"
            };
            string requestUriForPut = RequestBaseUri;

            // Act
            HttpResponseMessage response = await _client.PutAsJsonAsync(requestUriForPut, updateCommand);

            // Assert
            // Esto asume que tienes un validador para BrandUpdateCommand que exige que Id no sea nulo/vacío.
            // O que tu handler/controller tiene una comprobación explícita.
            // Si no hay tal validación y el handler intenta `Id.Value`, resultaría en un InternalServerError
            // debido a NullReferenceException, lo cual no es ideal.
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorDetails>(); // O ProblemDetails
            result.Should().NotBeNull();
            result!.Errors.Should().NotBeNullOrEmpty();
            // El nombre de la propiedad podría ser 'Id' o 'id' dependiendo de la serialización y configuración del validador
            result.Errors.Should().ContainSingle(e =>
                string.Equals(e.PropertyName, nameof(BrandUpdateCommand.Id), StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(e.ErrorMessage));
        }


        [Fact]
        public async Task WhenNameIsEmptyInUpdateCommand_ShouldReturnBadRequest()
        {
            // Arrange: Crear una marca primero
            var initialBrand = await CreateTestBrandAsync($"BrandToValidate NameEmpty {Guid.NewGuid()}", "Valid Initial Desc");
            initialBrand.Should().NotBeNull();
            initialBrand!.Id.Should().NotBeNull();

            var updateCommand = new BrandUpdateCommand()
            {
                Id = initialBrand.Id.Value,
                Name = string.Empty, // Nombre inválido
                Description = "Valid Description for update"
            };
            string requestUriForPut = RequestBaseUri;

            // Act
            HttpResponseMessage response = await _client.PutAsJsonAsync(requestUriForPut, updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            result.Should().NotBeNull();
            result!.Errors.Should().NotBeNullOrEmpty();
            result.Errors.Should().ContainSingle(e =>
                string.Equals(e.PropertyName, nameof(BrandUpdateCommand.Name), StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(e.ErrorMessage));
        }

        [Fact]
        public async Task WhenNameExceedsMaxLengthInUpdateCommand_ShouldReturnBadRequest()
        {
            // Arrange
            var initialBrand = await CreateTestBrandAsync($"BrandToValidate NameMax {Guid.NewGuid()}", "Valid Initial Desc");
            initialBrand.Should().NotBeNull();
            initialBrand!.Id.Should().NotBeNull();

            string nameTooLong = new('A', BaseCatalog.NameMaxLength + 1);
            var updateCommand = new BrandUpdateCommand()
            {
                Id = initialBrand.Id.Value,
                Name = nameTooLong,
                Description = "Valid Description for update"
            };
            string requestUriForPut = RequestBaseUri;

            // Act
            HttpResponseMessage response = await _client.PutAsJsonAsync(requestUriForPut, updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            result.Should().NotBeNull();
            result!.Errors.Should().NotBeNullOrEmpty();
            result.Errors.Should().ContainSingle(e =>
                string.Equals(e.PropertyName, nameof(BrandUpdateCommand.Name), StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(e.ErrorMessage));
        }
    }
}
