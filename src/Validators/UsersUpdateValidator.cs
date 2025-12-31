using System.Data;
using FluentValidation;
using UserService.src.DTOs;

namespace UserService.src.Validators
{
    public class UsersUpdateValidator : AbstractValidator<UsersUpdateDTO>
    {
        public UsersUpdateValidator()
        {
            RuleFor(u => u.Id)
                .NotEmpty().WithMessage("Por favor, digite o ID do utilizador.")
                .NotNull().WithMessage("O ID do utilizador é obrigatório e não pode estar vazio.");

            RuleFor(u => u.Fullname)
                .NotEmpty().WithMessage("Por favor, digite o nome completo.")
                .NotNull().WithMessage("O nome completo é obrigatório e não pode estar vazio.")
                .Length(2, 50).WithMessage("O nome completo deve ter entre 2 e 50 caracteres.");

            RuleFor(u => u.Username)
                .NotEmpty().WithMessage("Por favor, digite o nome de utilizador.")
                .NotNull().WithMessage("O nome de utilizador é obrigatório e não pode estar vazio.")
                .Length(10, 20).WithMessage("O nome de utilizador deve ter entre 10 e 20 caracteres.");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Por favor, digite o endereço de email.")
                .NotNull().WithMessage("O email é obrigatório e não pode estar vazio.")
                .EmailAddress().WithMessage("Por favor, digite um endereço de email válido.");

            RuleFor(u => u.RoleId)
                .NotEmpty().WithMessage("Por favor, digite a função.")
                .NotNull().WithMessage("A função é obrigatória e não pode estar vazia.");
        }
    }
}