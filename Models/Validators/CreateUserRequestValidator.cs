using FluentValidation;
using hh_napi.Configurations;
using Microsoft.Extensions.Options;

namespace hh_napi.Models.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator(IOptions<ValidationSettings> options)
    {
        var validationSettings = options.Value;

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(validationSettings.PasswordMinLength).WithMessage($"Password must be at least {validationSettings.PasswordMinLength} characters long.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");
    }
}