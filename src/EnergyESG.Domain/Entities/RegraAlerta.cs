namespace EnergyESG.Domain.Entities;

public class RegraAlerta
{
    public Guid Id { get; set; }
    public Guid UnidadeId { get; set; }
    public decimal LimiteKwhHora { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public string? Descricao { get; set; }
}


