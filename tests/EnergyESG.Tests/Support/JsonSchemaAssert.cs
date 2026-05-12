using System.Text.Json.Nodes;
using Json.Schema;
using Xunit;

namespace EnergyESG.Tests.Support;

public static class JsonSchemaAssert
{
    public static void Matches(string jsonBody, string schemaFileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "schemas", schemaFileName);
        Assert.True(File.Exists(path), $"Arquivo de schema não encontrado: {path}");

        var schemaText = File.ReadAllText(path);
        var schema = JsonSchema.FromText(schemaText);

        var trimmed = jsonBody.Trim();
        JsonNode instance;
        if (trimmed.StartsWith('{') || trimmed.StartsWith('['))
            instance = JsonNode.Parse(jsonBody)
                ?? throw new InvalidOperationException("Corpo JSON inválido.");
        else
            instance = JsonValue.Create(trimmed)
                ?? throw new InvalidOperationException("Corpo vazio.");

        var result = schema.Evaluate(instance, new EvaluationOptions());
        if (result.IsValid)
            return;

        Assert.Fail($"JSON Schema ({schemaFileName}): corpo não atende ao contrato.");
    }
}
