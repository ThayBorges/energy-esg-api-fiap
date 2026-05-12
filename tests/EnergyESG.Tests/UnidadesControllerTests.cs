using System.Net;
using System.Net.Http.Json;
using EnergyESG.Application.DTOs;
using EnergyESG.Tests.Support;
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
    public async Task Get_Returns200_AndPagedJsonMatchesSchema()
    {
        var response = await _client.GetAsync("/api/unidades");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        JsonSchemaAssert.Matches(body, "paged-unidades.json");
    }

    [Fact]
    public async Task Get_WithPagination_Returns200_AndMatchesSchema()
    {
        var response = await _client.GetAsync("/api/unidades?pagina=1&tamanho=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "paged-unidades.json");
    }

    [Fact]
    public async Task GetById_WhenMissing_Returns404()
    {
        var response = await _client.GetAsync($"/api/unidades/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_WhenInvalid_Returns400_AndValidationSchema()
    {
        var dto = new CreateUnidadeDto { Nome = "", Endereco = "Rua", Tipo = "Empresarial" };
        var response = await _client.PostAsJsonAsync("/api/unidades", dto);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "fluent-validation-errors.json");
    }

    [Fact]
    public async Task Post_WhenValid_Returns201_AndBodyMatchesSchema()
    {
        var dto = new CreateUnidadeDto
        {
            Nome = "Unidade integração",
            Endereco = "Rua Teste, 100",
            Tipo = "Empresarial"
        };

        var response = await _client.PostAsJsonAsync("/api/unidades", dto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.False(string.IsNullOrEmpty(response.Headers.Location?.ToString()));

        var body = await response.Content.ReadAsStringAsync();
        JsonSchemaAssert.Matches(body, "unidade-vm.json");
    }

    [Fact]
    public async Task GetById_WhenExists_Returns200_AndMatchesSchema()
    {
        var create = new CreateUnidadeDto
        {
            Nome = "Para consulta por id",
            Endereco = "Av. Central",
            Tipo = "Industrial"
        };
        var created = await _client.PostAsJsonAsync("/api/unidades", create);
        created.EnsureSuccessStatusCode();
        var vm = await created.Content.ReadFromJsonAsync<EnergyESG.Application.ViewModels.UnidadeVm>();
        Assert.NotNull(vm);

        var response = await _client.GetAsync($"/api/unidades/{vm!.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        JsonSchemaAssert.Matches(await response.Content.ReadAsStringAsync(), "unidade-vm.json");
    }
}
