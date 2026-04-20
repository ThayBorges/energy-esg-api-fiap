namespace EnergyESG.Domain.Entities;

public class ConsumoEnergia
{
    public Guid Id { get; set; }
    public Guid UnidadeId { get; set; }
    public DateTime DataHora { get; set; }
    public decimal Kwh { get; set; }
    public bool UltrapassouLimite { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}


