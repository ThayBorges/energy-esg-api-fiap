using System.Net;
using EnergyESG.Domain.Entities;
using EnergyESG.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EnergyESG.Tests;

public class RelatoriosControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public RelatoriosControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Energia_ReturnsHttpStatusCode200()
    {
        // Arrange
        var unidadeId = Guid.NewGuid();
        var de = DateTime.UtcNow.AddDays(-30);
        var ate = DateTime.UtcNow;

        // O endpoint retorna 404 quando a unidade não existe;
        // por isso criamos uma unidade de teste antes da requisição.
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<EnergyContext>();
            context.Unidades.Add(new Unidade
            {
                Id = unidadeId,
                Nome = "Unidade Teste Relatorio"
            });
            await context.SaveChangesAsync();
        }

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


