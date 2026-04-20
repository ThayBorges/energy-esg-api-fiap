using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using EnergyESG.Infrastructure.Data;

namespace EnergyESG.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext real
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<EnergyContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Adiciona DbContext em memória para testes
            services.AddDbContext<EnergyContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Remove autenticação JWT
            var authDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(Microsoft.AspNetCore.Authentication.IAuthenticationService));

            // Configura autenticação de teste
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test", options => { });
        });

        builder.UseEnvironment("Testing");
    }
}

