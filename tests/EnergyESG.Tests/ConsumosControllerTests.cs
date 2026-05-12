using System.Net;
using System.Net.Http.Json;
using EnergyESG.Application.DTOs;
using EnergyESG.Application.ViewModels;
using EnergyESG.Tests.Support;
using Xunit;

namespace EnergyESG.Tests;

public class ConsumosControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ConsumosControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Returns200_AndPagedSchema()
    {
        var response = await _client.GetAsync("/api/consumos");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "paged-consumos.json");
    }

    [Fact]
    public async Task Get_WithPagination_Returns200_AndPagedSchema()
    {
        var response = await _client.GetAsync("/api/consumos?pagina=1&tamanho=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "paged-consumos.json");
    }

    [Fact]
    public async Task GetById_WhenMissing_Returns404()
    {
        var response = await _client.GetAsync($"/api/consumos/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_WhenUnidadeInvalid_Returns400_AndMessageSchema()
    {
        var dto = new CreateConsumoDto
        {
            UnidadeId = Guid.NewGuid(),
            DataHora = DateTime.UtcNow,
            Kwh = 10m
        };
        var response = await _client.PostAsJsonAsync("/api/consumos", dto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "texto-erro-api.json");
    }

    [Fact]
    public async Task Post_WhenValidationFails_Returns400_AndErrorsSchema()
    {
        var unidadeId = await DbSeed.SeedUnidadeAsync(_factory.Services);
        var dto = new CreateConsumoDto
        {
            UnidadeId = unidadeId,
            DataHora = DateTime.UtcNow,
            Kwh = 0m
        };
        var response = await _client.PostAsJsonAsync("/api/consumos", dto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "fluent-validation-errors.json");
    }

    [Fact]
    public async Task Post_WhenValid_Returns201_AndConsumoSchema()
    {
        var unidadeId = await DbSeed.SeedUnidadeAsync(_factory.Services);
        var dto = new CreateConsumoDto
        {
            UnidadeId = unidadeId,
            DataHora = DateTime.UtcNow,
            Kwh = 42.5m
        };
        var response = await _client.PostAsJsonAsync("/api/consumos", dto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "consumo-vm.json");
    }

    [Fact]
    public async Task GetById_WhenExists_Returns200_AndSchema()
    {
        var unidadeId = await DbSeed.SeedUnidadeAsync(_factory.Services);
        var created = await _client.PostAsJsonAsync("/api/consumos", new CreateConsumoDto
        {
            UnidadeId = unidadeId,
            DataHora = DateTime.UtcNow,
            Kwh = 12m
        });
        created.EnsureSuccessStatusCode();
        var vm = await created.Content.ReadFromJsonAsync<ConsumoVm>();
        Assert.NotNull(vm);

        var response = await _client.GetAsync($"/api/consumos/{vm!.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "consumo-vm.json");
    }
}
