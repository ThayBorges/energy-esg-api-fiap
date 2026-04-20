namespace EnergyESG.Application.DTOs;

public class CreateConsumoDto
{
    public Guid UnidadeId { get; set; }
    public DateTime DataHora { get; set; }
    public decimal Kwh { get; set; }
}


