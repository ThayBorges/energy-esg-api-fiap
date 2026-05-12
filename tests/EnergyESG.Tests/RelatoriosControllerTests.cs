using System.Net;
using EnergyESG.Domain.Entities;
using EnergyESG.Infrastructure.Data;
using EnergyESG.Tests.Support;
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
    public async Task Energia_WhenDatesInvalid_Returns400_AndMessageSchema()
    {
        var de = DateTime.UtcNow;
        var ate = DateTime.UtcNow.AddDays(-1);
        var url = $"/api/relatorios/energia?unidadeId={Guid.NewGuid()}&de={de:yyyy-MM-dd}&ate={ate:yyyy-MM-dd}";
        var response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "texto-erro-api.json");
    }

    [Fact]
    public async Task Energia_WhenUnidadeMissing_Returns404_AndMessageSchema()
    {
        var de = DateTime.UtcNow.AddDays(-10);
        var ate = DateTime.UtcNow;
        var url = $"/api/relatorios/energia?unidadeId={Guid.NewGuid()}&de={de:yyyy-MM-dd}&ate={ate:yyyy-MM-dd}";
        var response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "texto-erro-api.json");
    }

    [Fact]
    public async Task Energia_WhenUnidadeExists_Returns200_AndSchema()
    {
        var unidadeId = Guid.NewGuid();
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<EnergyContext>();
            context.Unidades.Add(new Unidade
            {
                Id = unidadeId,
                Nome = "Unidade relatório energia",
                Tipo = "Comercial",
                Ativo = true
            });
            await context.SaveChangesAsync();
        }

        var de = DateTime.UtcNow.AddDays(-30);
        var ate = DateTime.UtcNow;
        var url = $"/api/relatorios/energia?unidadeId={unidadeId}&de={de:yyyy-MM-dd}&ate={ate:yyyy-MM-dd}";
        var response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "relatorio-energia-vm.json");
    }

    [Fact]
    public async Task ConsumoPorPeriodo_WhenDatesInvalid_Returns400_AndMessageSchema()
    {
        var de = DateTime.UtcNow;
        var ate = DateTime.UtcNow.AddDays(-2);
        var url =
            $"/api/relatorios/consumo-por-periodo?unidadeId={Guid.NewGuid()}&de={de:yyyy-MM-dd}&ate={ate:yyyy-MM-dd}&agrupamento=dia";
        var response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "texto-erro-api.json");
    }

    [Fact]
    public async Task ConsumoPorPeriodo_WhenValid_Returns200_AndArraySchema()
    {
        var unidadeId = Guid.NewGuid();
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<EnergyContext>();
            context.Unidades.Add(new Unidade
            {
                Id = unidadeId,
                Nome = "Unidade relatório período",
                Ativo = true
            });
            await context.SaveChangesAsync();
        }

        var de = DateTime.UtcNow.AddDays(-7);
        var ate = DateTime.UtcNow;
        var url =
            $"/api/relatorios/consumo-por-periodo?unidadeId={unidadeId}&de={de:yyyy-MM-dd}&ate={ate:yyyy-MM-dd}&agrupamento=dia";
        var response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "relatorio-consumo-periodo-item.json");
    }

    [Fact]
    public async Task ConsumoPorPeriodo_WhenAgrupamentoMes_Returns200_AndArraySchema()
    {
        var unidadeId = Guid.NewGuid();
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<EnergyContext>();
            context.Unidades.Add(new Unidade { Id = unidadeId, Nome = "U mes", Ativo = true });
            await context.SaveChangesAsync();
        }

        var de = DateTime.UtcNow.AddMonths(-2);
        var ate = DateTime.UtcNow;
        var url =
            $"/api/relatorios/consumo-por-periodo?unidadeId={unidadeId}&de={de:yyyy-MM-dd}&ate={ate:yyyy-MM-dd}&agrupamento=mes";
        var response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "relatorio-consumo-periodo-item.json");
    }
}
