using ContestSystem.GatewayApi.Auth.Contracts;
using FluentValidation;

namespace ContestSystem.GatewayApi.Auth.Validators;

public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
{
    public SignUpRequestValidator()
    {
        RuleFor(r => r.Username)
            .NotEmpty()
            .WithMessage("Username can't be null, empty or whitespace");

        RuleFor(r => r.Password)
            .NotEmpty()
            .WithMessage("Password can't be null, empty or whitespace");

        RuleFor(r => r.PasswordRepeat)
            .Equal(r => r.Password)
            .WithMessage("Password repeat should be the same as password");
    }
}
