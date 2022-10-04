using ContestSystem.GatewayApi.Auth.Contracts;
using ContestSystem.GatewayApi.Auth.Validators;
using FluentAssertions;

namespace ContestSystem.GatewayApi.Tests.Unit.Auth;

public class SignUpEndpointTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(null)]
    public async Task Validator_ShouldFail_WhenUsernameIsEmpty(string? username)
    {
        // Assign
        var request = new SignUpRequest
        {
            Username = username,
            Password = "password",
            PasswordRepeat = "password"
        };

        var validator = new SignUpRequestValidator();
        
        // Act
        var result = await validator.ValidateAsync(request);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors[0].PropertyName.Should().BeOfType<string>()
            .And.BeEquivalentTo("Username");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(null)]
    public async Task Validator_ShouldFail_WhenPasswordIsEmpty(string? password)
    {
        // Assign
        var request = new SignUpRequest
        {
            Username = "username",
            Password = password,
            PasswordRepeat = password
        };

        var validator = new SignUpRequestValidator();
        
        // Act
        var result = await validator.ValidateAsync(request);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors[0].PropertyName.Should().BeOfType<string>()
            .And.BeEquivalentTo("Password");
    }

    [Fact]
    public async Task Validator_ShouldFail_WhenPasswordRepeatIsNotPassword()
    {
        // Assign
        var request = new SignUpRequest
        {
            Username = "username",
            Password = "password",
            PasswordRepeat = "not-password"
        };

        var validator = new SignUpRequestValidator();
        
        // Act
        var result = await validator.ValidateAsync(request);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors[0].PropertyName.Should().BeOfType<string>()
            .And.BeEquivalentTo("PasswordRepeat");
    }
}
