using FluentValidation;
using UserService.src.DTOs;

namespace UserService.src.Validators
{
    public class RolesUpdateValidator : AbstractValidator<RolesDTO>
    {
        public RolesUpdateValidator()
        {
            RuleFor(r => r.Id)
                .NotEmpty().WithMessage("Por favor, informe o ID da função.")
                .NotNull().WithMessage("O ID da função é obrigatório e não pode estar vazio.");

            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("Por favor, digite o nome da função.")
                .NotNull().WithMessage("O nome da função é obrigatório e não pode estar vazio.");
        }
    }
}