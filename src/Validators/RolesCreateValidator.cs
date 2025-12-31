using FluentValidation;
using UserService.src.DTOs;

namespace UserService.src.Validators
{
    public sealed class RolesCreateValidator : AbstractValidator<RolesCreateDTO>
    {
        public RolesCreateValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("Por favor, digite o nome da função.")
                .NotNull().WithMessage("O nome da função é obrigatório e não pode estar vazio.");
        }        
    }
}