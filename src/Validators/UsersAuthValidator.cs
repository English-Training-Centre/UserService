using FluentValidation;
using UserService.src.DTOs;

namespace UserService.src.Validators
{
    public class UsersAuthValidator : AbstractValidator<AuthUsersDTO>
    {
        public UsersAuthValidator()
        {
            RuleFor(u => u.Username)
                .NotEmpty().WithMessage("Invalid credentials.")
                .NotNull().WithMessage("Invalid credentials.");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Invalid credentials.")
                .NotNull().WithMessage("Invalid credentials.");
        }
    }
}