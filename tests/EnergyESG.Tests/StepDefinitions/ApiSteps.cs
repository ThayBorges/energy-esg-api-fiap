using System.Net;
using System.Text;
using EnergyESG.Tests.Support;
using Reqnroll;
using Xunit;

namespace EnergyESG.Tests.StepDefinitions;

[Binding]
public sealed class ApiSteps
{
    private readonly ScenarioContext _scenario;

    public ApiSteps(ScenarioContext scenarioContext)
    {
        _scenario = scenarioContext;
    }

    private HttpClient Client => (HttpClient)_scenario["client"]!;

    [When("solicito GET {string}")]
    public async Task WhenGetAsync(string pathAndQuery)
    {
        var response = await Client.GetAsync(pathAndQuery);
        _scenario["lastResponse"] = response;
        _scenario["lastBody"] = await response.Content.ReadAsStringAsync();
    }

    [When("envio POST {string} com o JSON:")]
    public async Task WhenPostJsonAsync(string path, string json)
    {
        using var content = new StringContent(json.Trim(), Encoding.UTF8, "application/json");
        var response = await Client.PostAsync(path, content);
        _scenario["lastResponse"] = response;
        _scenario["lastBody"] = await response.Content.ReadAsStringAsync();
    }

    [Then("o status HTTP deve ser {int}")]
    public void ThenStatus(int expected)
    {
        var response = (HttpResponseMessage)_scenario["lastResponse"]!;
        Assert.Equal((HttpStatusCode)expected, response.StatusCode);
    }

    [Then("o corpo deve validar o schema {string}")]
    public void ThenBodyMatchesSchema(string schemaFileName)
    {
        var body = (string)_scenario["lastBody"]!;
        JsonSchemaAssert.Matches(body, schemaFileName);
    }
}
