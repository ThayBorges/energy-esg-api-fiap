using FluentValidation;
using EnergyESG.Application.DTOs;

namespace EnergyESG.Application.Validations;

public class CreateRegraAlertaValidator : AbstractValidator<CreateRegraAlertaDto>
{
    public CreateRegraAlertaValidator()
    {
        RuleFor(x => x.UnidadeId)
            .NotEmpty().WithMessage("UnidadeId é obrigatório");

        RuleFor(x => x.LimiteKwhHora)
            .GreaterThan(0).WithMessage("LimiteKwhHora deve ser maior que zero")
            .LessThan(100000).WithMessage("LimiteKwhHora deve ser menor que 100000");
    }
}


