namespace EnergyESG.Application.ViewModels;

public class ConsumoVm
{
    public Guid Id { get; set; }
    public Guid UnidadeId { get; set; }
    public DateTime DataHora { get; set; }
    public decimal Kwh { get; set; }
    public bool UltrapassouLimite { get; set; }
}


