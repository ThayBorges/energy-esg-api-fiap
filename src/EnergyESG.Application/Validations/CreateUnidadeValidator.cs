using FluentValidation;
using EnergyESG.Application.DTOs;

namespace EnergyESG.Application.Validations;

public class CreateUnidadeValidator : AbstractValidator<CreateUnidadeDto>
{
    public CreateUnidadeValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Tipo)
            .MaximumLength(50).WithMessage("Tipo deve ter no máximo 50 caracteres");
    }
}


