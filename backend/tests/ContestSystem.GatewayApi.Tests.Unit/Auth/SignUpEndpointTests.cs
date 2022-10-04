using ContestSystem.GatewayApi.Auth.Contracts;
using ContestSystem.GatewayApi.Auth.Endpoints;
using ContestSystem.GatewayApi.Auth.Mappers;
using ContestSystem.GatewayApi.Auth.Models;
using ContestSystem.GatewayApi.Auth.Services;
using ContestSystem.GatewayApi.Auth.Validators;
using ContestSystem.GatewayApi.Common.Interfaces;
using ContestSystem.GatewayApi.Common.Services;

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

    [Fact]
    public async Task HandleAsync_ShouldReturnUnsuccessfulResponse_WhenSignUpFails()
    {
        // Assign
        var authService = Substitute.For<IAuthService>();
        authService.SignUpAsync(new SignUpData(), default)
            .ReturnsForAnyArgs(Task.FromResult(new SignUpResult
                {
                    Success = false,
                    Errors = { "SomeError" }
                }));

        var requestMapper = new SignUpRequestMapper();
        var responseMapper = new SignUpResponseMapper(new IdsHasher());
        
        var endpoint = Factory.Create<SignUpEndpoint>(
            authService,
            requestMapper,
            responseMapper);
        
        var request = new SignUpRequest
        {
            Username = "username",
            Password = "password",
            PasswordRepeat = "password"
        };

        var tokenSource = new CancellationTokenSource();
        
        // Act
        await endpoint.HandleAsync(request, tokenSource.Token);
        
        // Assert
        var response = endpoint.Response;
        response.Success.Should().BeFalse();
        response.Errors.Should().ContainSingle();
        response.Errors[0].Should().BeEquivalentTo("SomeError");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccessfulResponse_WhenSignUpPasses()
    {
        // Assign
        var idsHasher = new IdsHasher() as IIdsHasher;
        var authService = Substitute.For<IAuthService>();
        authService.SignUpAsync(new SignUpData(), default)
            .ReturnsForAnyArgs(Task.FromResult(new SignUpResult
                {
                    Success = true,
                    UserId = 1
                }));

        var requestMapper = new SignUpRequestMapper();
        var responseMapper = new SignUpResponseMapper(idsHasher);
        
        var endpoint = Factory.Create<SignUpEndpoint>(
            authService,
            requestMapper,
            responseMapper);
        
        var request = new SignUpRequest
        {
            Username = "username",
            Password = "password",
            PasswordRepeat = "password"
        };

        var tokenSource = new CancellationTokenSource();
        
        // Act
        await endpoint.HandleAsync(request, tokenSource.Token);
        
        // Assert
        var response = endpoint.Response;
        response.Success.Should().BeTrue();
        response.Errors.Should().BeEmpty();

        idsHasher.DecodeUserId(response.UserId)
            .Should().Be(1);
    }
}
