using System.Net;
using EnergyESG.Tests.Support;
using Xunit;

namespace EnergyESG.Tests;

public class SensoresControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public SensoresControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Returns200_AndListSchema()
    {
        var response = await _client.GetAsync("/api/sensores");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "sensor-list-item.json");
    }

    [Fact]
    public async Task Get_WithUnidadeFilter_Returns200_AndListSchema()
    {
        var unidadeId = await DbSeed.SeedUnidadeAsync(_factory.Services);
        var response = await _client.GetAsync($"/api/sensores?unidadeId={unidadeId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "sensor-list-item.json");
    }

    [Fact]
    public async Task Desligar_WhenSensorMissing_Returns404_AndMessageSchema()
    {
        var response = await _client.PostAsync($"/api/sensores/{Guid.NewGuid()}/acoes/desligar", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "texto-erro-api.json");
    }

    [Fact]
    public async Task Desligar_WhenActiveSensor_Returns200_AndActionSchema()
    {
        var unidadeId = await DbSeed.SeedUnidadeAsync(_factory.Services);
        var sensorId = await DbSeed.SeedSensorAsync(_factory.Services, unidadeId, ativo: true);

        var response = await _client.PostAsync($"/api/sensores/{sensorId}/acoes/desligar", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "sensor-acao-resposta.json");
    }

    [Fact]
    public async Task Ligar_WhenSensorMissing_Returns404_AndMessageSchema()
    {
        var response = await _client.PostAsync($"/api/sensores/{Guid.NewGuid()}/acoes/ligar", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "texto-erro-api.json");
    }

    [Fact]
    public async Task Ligar_WhenSensorExists_Returns200_AndActionSchema()
    {
        var unidadeId = await DbSeed.SeedUnidadeAsync(_factory.Services);
        var sensorId = await DbSeed.SeedSensorAsync(_factory.Services, unidadeId);

        var response = await _client.PostAsync($"/api/sensores/{sensorId}/acoes/ligar", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "sensor-acao-resposta.json");
    }
}
