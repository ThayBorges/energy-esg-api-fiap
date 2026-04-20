namespace EnergyESG.Domain.Entities;

public class Sensor
{
    public Guid Id { get; set; }
    public Guid UnidadeId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // Energia, Temperatura, etc.
    public bool Ativo { get; set; } = true;
    public bool DesligadoAutomaticamente { get; set; } = false;
    public DateTime? UltimaAcao { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}


