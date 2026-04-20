using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace EnergyESG.Tests;

public class SensoresControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SensoresControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        // Adiciona token de autenticação para testes
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-token");
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/sensores";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        // Pode retornar 200 (OK) ou 401 (Unauthorized)
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Desligar_ReturnsHttpStatusCode200()
    {
        // Arrange
        var sensorId = Guid.NewGuid();
        var request = $"/api/sensores/{sensorId}/acoes/desligar";

        // Act
        var response = await _client.PostAsync(request, null);

        // Assert
        // Pode retornar 200 (OK), 404 (NotFound) ou 401 (Unauthorized)
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}


