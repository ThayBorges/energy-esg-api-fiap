using FluentValidation;
using EnergyESG.Application.DTOs;

namespace EnergyESG.Application.Validations;

public class CreateConsumoValidator : AbstractValidator<CreateConsumoDto>
{
    public CreateConsumoValidator()
    {
        RuleFor(x => x.UnidadeId)
            .NotEmpty().WithMessage("UnidadeId é obrigatório");

        RuleFor(x => x.DataHora)
            .NotEmpty().WithMessage("DataHora é obrigatória")
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
            .WithMessage("DataHora não pode ser no futuro");

        RuleFor(x => x.Kwh)
            .GreaterThan(0).WithMessage("Kwh deve ser maior que zero")
            .LessThan(100000).WithMessage("Kwh deve ser menor que 100000");
    }
}


