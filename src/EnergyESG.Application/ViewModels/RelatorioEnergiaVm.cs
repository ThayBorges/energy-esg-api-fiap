namespace EnergyESG.Application.ViewModels;

public class RelatorioEnergiaVm
{
    public Guid UnidadeId { get; set; }
    public decimal TotalKwh { get; set; }
    public int Picos { get; set; }
    public decimal MediaKwh { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
}


