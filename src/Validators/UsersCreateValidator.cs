using FluentValidation;
using UserService.src.DTOs;

namespace UserService.src.Validators
{
    public class UsersCreateValidator : AbstractValidator<UsersCreateDTO>
    {
        public UsersCreateValidator()
        {
            RuleFor(u => u.Fullname)
                .NotEmpty().WithMessage("Por favor, digite o nome completo.")
                .NotNull().WithMessage("O nome completo é obrigatório e não pode estar vazio.")
                .Length(2, 50).WithMessage("O nome completo deve ter entre 2 e 50 caracteres.");

            RuleFor(u => u.Username)
                .NotEmpty().WithMessage("Por favor, digite o nome de utilizador.")
                .NotNull().WithMessage("O nome de utilizador é obrigatório e não pode estar vazio.")
                .Length(10, 20).WithMessage("O nome de utilizador deve ter entre 10 e 20 caracteres.");

            RuleFor(u => u.PhoneNumber)
                .NotEmpty().WithMessage("Por favor, digite o número de telefone.")
                .NotNull().WithMessage("O nr. telefone é obrigatório e não pode estar vazio.");

            RuleFor(u => u.RoleId)
                .NotEmpty().WithMessage("Por favor, digite a função.")
                .NotNull().WithMessage("A função é obrigatória e não pode estar vazia.");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Por favor, digite a palavra-passe.")
                .NotNull().WithMessage("A palavra-passe é obrigatória e não pode estar vazia.")
                .Length(6, 50).WithMessage("A palavra-passe deve ter entre 6 e 50 caracteres.");
        }
    }
}