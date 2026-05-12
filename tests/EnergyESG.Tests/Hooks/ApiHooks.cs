using Reqnroll;

namespace EnergyESG.Tests.Hooks;

[Binding]
public sealed class ApiHooks
{
    private readonly ScenarioContext _scenario;

    public ApiHooks(ScenarioContext scenarioContext)
    {
        _scenario = scenarioContext;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        var factory = new CustomWebApplicationFactory();
        _scenario["factory"] = factory;
        _scenario["client"] = factory.CreateClient();
    }

    [AfterScenario]
    public void AfterScenario()
    {
        if (_scenario.TryGetValue("factory", out var f) && f is CustomWebApplicationFactory cf)
            cf.Dispose();
    }
}
