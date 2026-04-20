namespace EnergyESG.Application.DTOs;

public class CreateRegraAlertaDto
{
    public Guid UnidadeId { get; set; }
    public decimal LimiteKwhHora { get; set; }
    public string? Descricao { get; set; }
}


