using System.Net;
using System.Net.Http.Json;
using EnergyESG.Application.DTOs;
using EnergyESG.Application.ViewModels;
using EnergyESG.Tests.Support;
using Xunit;

namespace EnergyESG.Tests;

public class AlertasControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AlertasControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Returns200_AndListSchema()
    {
        var unidadeId = await DbSeed.SeedUnidadeAsync(_factory.Services);
        var response = await _client.GetAsync($"/api/alertas?unidadeId={unidadeId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "regra-alerta-lista.json");
    }

    [Fact]
    public async Task Post_WhenValidationFails_Returns400_AndErrorsSchema()
    {
        var dto = new CreateRegraAlertaDto
        {
            UnidadeId = Guid.Empty,
            LimiteKwhHora = 10m,
            Descricao = "x"
        };
        var response = await _client.PostAsJsonAsync("/api/alertas", dto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "fluent-validation-errors.json");
    }

    [Fact]
    public async Task Post_WhenUnidadeInvalid_Returns400_AndMessageSchema()
    {
        var dto = new CreateRegraAlertaDto
        {
            UnidadeId = Guid.NewGuid(),
            LimiteKwhHora = 40m,
            Descricao = "Regra órfã"
        };
        var response = await _client.PostAsJsonAsync("/api/alertas", dto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "texto-erro-api.json");
    }

    [Fact]
    public async Task Post_WhenValid_Returns201_AndVmSchema()
    {
        var unidadeId = await DbSeed.SeedUnidadeAsync(_factory.Services);
        var dto = new CreateRegraAlertaDto
        {
            UnidadeId = unidadeId,
            LimiteKwhHora = 55m,
            Descricao = "Limite horário"
        };
        var response = await _client.PostAsJsonAsync("/api/alertas", dto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "regra-alerta-vm.json");
    }

    [Fact]
    public async Task Get_AfterPost_ReturnsActiveRuleInList()
    {
        var unidadeId = await DbSeed.SeedUnidadeAsync(_factory.Services);
        var created = await _client.PostAsJsonAsync("/api/alertas", new CreateRegraAlertaDto
        {
            UnidadeId = unidadeId,
            LimiteKwhHora = 33m,
            Descricao = "Monitoramento"
        });
        created.EnsureSuccessStatusCode();

        var response = await _client.GetAsync($"/api/alertas?unidadeId={unidadeId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        JsonSchemaAssert.Matches(body, "regra-alerta-lista.json");
        var list = await response.Content.ReadFromJsonAsync<List<RegraAlertaVm>>();
        Assert.NotNull(list);
        Assert.Contains(list!, r => r.UnidadeId == unidadeId && r.Ativo);
    }
}
