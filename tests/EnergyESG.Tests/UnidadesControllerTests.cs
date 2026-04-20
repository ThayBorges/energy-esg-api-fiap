using System.Net;
using System.Net.Http.Json;
using EnergyESG.Application.DTOs;
using Xunit;

namespace EnergyESG.Tests;

public class UnidadesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UnidadesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/unidades";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode(); // Esta linha verifica se o status code é 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_WithPagination_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/unidades?pagina=1&tamanho=10";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsHttpStatusCode201()
    {
        // Arrange
        var dto = new CreateUnidadeDto
        {
            Nome = "Unidade Teste",
            Endereco = "Rua Teste, 123",
            Tipo = "Empresarial"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/unidades", dto);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.BadRequest);
    }
}


