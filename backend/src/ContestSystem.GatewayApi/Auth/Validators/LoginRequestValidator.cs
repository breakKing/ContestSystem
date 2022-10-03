using ContestSystem.GatewayApi.Auth.Contracts;

namespace ContestSystem.GatewayApi.Auth.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.Login).NotEmpty()
            .WithMessage("Login can't be null, empty or whitespace");
    }
}
