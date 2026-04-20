using Microsoft.EntityFrameworkCore;
using EnergyESG.Domain.Entities;

namespace EnergyESG.Infrastructure.Data;

public class EnergyContext : DbContext
{
    public EnergyContext(DbContextOptions<EnergyContext> options) : base(options)
    {
    }

    public DbSet<ConsumoEnergia> Consumos { get; set; }
    public DbSet<RegraAlerta> RegrasAlertas { get; set; }
    public DbSet<Unidade> Unidades { get; set; }
    public DbSet<Sensor> Sensores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ConsumoEnergia
        modelBuilder.Entity<ConsumoEnergia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UnidadeId, e.DataHora });
            entity.Property(e => e.Kwh).HasColumnType("decimal(18,4)");
            entity.Property(e => e.DataHora).IsRequired();
        });

        // RegraAlerta
        modelBuilder.Entity<RegraAlerta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UnidadeId, e.Ativo });
            entity.Property(e => e.LimiteKwhHora).HasColumnType("decimal(18,4)");
        });

        // Unidade
        modelBuilder.Entity<Unidade>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Tipo).HasMaxLength(50);
        });

        // Sensor
        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UnidadeId, e.Ativo });
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Tipo).HasMaxLength(50);
        });
    }
}


