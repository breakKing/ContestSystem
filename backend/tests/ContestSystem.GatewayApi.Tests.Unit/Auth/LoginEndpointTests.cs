using ContestSystem.GatewayApi.Auth.Contracts;
using ContestSystem.GatewayApi.Auth.Endpoints;
using ContestSystem.GatewayApi.Auth.Mappers;
using ContestSystem.GatewayApi.Auth.Models;
using ContestSystem.GatewayApi.Auth.Services;
using ContestSystem.GatewayApi.Auth.Validators;
using FastEndpoints;
using FluentAssertions;
using NSubstitute;

namespace ContestSystem.GatewayApi.Tests.Unit.Auth;

public class LoginEndpointTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(null)]
    public async Task Validator_ShouldFail_WhenLoginIsEmpty(string? login)
    {
        // Assign
        var request = new LoginRequest
        {
            Login = login,
            Password = "password"
        };

        var validator = new LoginRequestValidator();
        
        // Act
        var result = await validator.ValidateAsync(request);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors[0].PropertyName.Should().BeOfType<string>()
            .And.BeEquivalentTo("Login");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUnsuccessfulResponse_WhenLoginFails()
    {
        // Assign
        var authService = Substitute.For<IAuthService>();
        authService.LoginAsync(new LoginCredentials(), default)
            .ReturnsForAnyArgs(Task.FromResult(new LoginResult
                {
                    Success = false,
                    Errors = { "SomeError" }
                }));

        var requestMapper = new LoginRequestMapper();
        var responseMapper = new LoginResponseMapper();
        
        var endpoint = Factory.Create<LoginEndpoint>(
            authService,
            requestMapper,
            responseMapper);
        
        var request = new LoginRequest
        {
            Login = "login",
            Password = "password"
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
    public async Task HandleAsync_ShouldReturnSuccessfulResponse_WhenLoginPasses()
    {
        // Assign
        var now = DateTimeOffset.UtcNow;
        var authService = Substitute.For<IAuthService>();
        authService.LoginAsync(new LoginCredentials(), default)
            .ReturnsForAnyArgs(Task.FromResult(new LoginResult
                {
                    Success = true,
                    AccessToken = "access-token",
                    RefreshToken = "refresh-token",
                    ExpiresAt = now
                }));

        var requestMapper = new LoginRequestMapper();
        var responseMapper = new LoginResponseMapper();
        
        var endpoint = Factory.Create<LoginEndpoint>(
            authService,
            requestMapper,
            responseMapper);
        
        var request = new LoginRequest
        {
            Login = "login",
            Password = "password"
        };

        var tokenSource = new CancellationTokenSource();
        
        // Act
        await endpoint.HandleAsync(request, tokenSource.Token);
        
        // Assert
        var response = endpoint.Response;
        response.Success.Should().BeTrue();
        response.Errors.Should().BeEmpty();
        response.AccessToken.Should().BeEquivalentTo("access-token");
        response.ExpiresAt.Should().BeExactly(now);
    }
}
