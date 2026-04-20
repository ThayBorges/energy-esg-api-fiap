namespace EnergyESG.Application.ViewModels;

public class RegraAlertaVm
{
    public Guid Id { get; set; }
    public Guid UnidadeId { get; set; }
    public decimal LimiteKwhHora { get; set; }
    public bool Ativo { get; set; }
    public string? Descricao { get; set; }
}


