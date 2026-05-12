using EnergyESG.Domain.Entities;
using EnergyESG.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace EnergyESG.Tests.Support;

internal static class DbSeed
{
    public static async Task<Guid> SeedUnidadeAsync(IServiceProvider services, string nome = "Unidade seed")
    {
        await using var scope = services.CreateAsyncScope();
        var ctx = scope.ServiceProvider.GetRequiredService<EnergyContext>();
        var id = Guid.NewGuid();
        ctx.Unidades.Add(new Unidade
        {
            Id = id,
            Nome = nome,
            Endereco = "Endereço seed",
            Tipo = "Empresarial",
            Ativo = true
        });
        await ctx.SaveChangesAsync();
        return id;
    }

    public static async Task<Guid> SeedSensorAsync(IServiceProvider services, Guid unidadeId, bool ativo = true)
    {
        await using var scope = services.CreateAsyncScope();
        var ctx = scope.ServiceProvider.GetRequiredService<EnergyContext>();
        var id = Guid.NewGuid();
        ctx.Sensores.Add(new Sensor
        {
            Id = id,
            UnidadeId = unidadeId,
            Nome = "Sensor seed",
            Tipo = "Energia",
            Ativo = ativo,
            DesligadoAutomaticamente = false
        });
        await ctx.SaveChangesAsync();
        return id;
    }
}
