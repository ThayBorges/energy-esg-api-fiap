namespace EnergyESG.Application.ViewModels;

public class UnidadeVm
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public string? Tipo { get; set; }
    public bool Ativo { get; set; }
}


