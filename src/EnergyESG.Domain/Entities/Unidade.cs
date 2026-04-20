namespace EnergyESG.Domain.Entities;

public class Unidade
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public string? Tipo { get; set; } // Empresa, Residencial, Industrial, etc.
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}


