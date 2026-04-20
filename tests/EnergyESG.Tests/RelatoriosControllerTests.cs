using System.Net;
using Xunit;

namespace EnergyESG.Tests;

public class RelatoriosControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RelatoriosControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Energia_ReturnsHttpStatusCode200()
    {
        // Arrange
        var unidadeId = Guid.NewGuid();
        var de = DateTime.UtcNow.AddDays(-30);
        var ate = DateTime.UtcNow;
        var request = $"/api/relatorios/energia?unidadeId={unidadeId}&de={de:yyyy-MM-dd}&ate={ate:yyyy-MM-dd}";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode(); // Esta linha verifica se o status code é 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ConsumoPorPeriodo_ReturnsHttpStatusCode200()
    {
        // Arrange
        var unidadeId = Guid.NewGuid();
        var de = DateTime.UtcNow.AddDays(-30);
        var ate = DateTime.UtcNow;
        var request = $"/api/relatorios/consumo-por-periodo?unidadeId={unidadeId}&de={de:yyyy-MM-dd}&ate={ate:yyyy-MM-dd}&agrupamento=dia";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}


