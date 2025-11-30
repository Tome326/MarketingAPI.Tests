using System.Net;
using System.Net.Http.Json;
using MarketingAPI.Models.DTOs;

namespace MarketingAPI.Test;

[Collection("API Tests")]
public class AuthAPITest
{
    private readonly HttpClient _client;
    private readonly string _jwtToken;

    public AuthAPITest(AuthenticationFixture fixture)
    {
        _client = fixture.Client;
        _jwtToken = fixture.JwtToken;
    }

    [Fact]
    public async Task Login_IncorrectPassword_ReturnUnauthorized()
    {
        LoginDto dto = new()
        {
            Username = "test",
            Password = "test123"
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/login", dto, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_IncorrectUsername_ReturnUnauthorized()
    {
        LoginDto dto = new()
        {
            Username = "testt",
            Password = "test123!"
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/login", dto, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateUsername_ReturnBadRequest()
    {
        RegisterDto dto = new()
        {
            Username = "test",
            Password = "UniquePassword123!",
            Email = "unique@email.com"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/register", dto, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnBadRequest()
    {
        RegisterDto dto = new()
        {
            Username = "unique user",
            Password = "UniquePassword123!",
            Email = "test@email.com"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/register", dto, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

/*
        _client.DefaultRequestHeaders.Authorization = null;
        CustomerDto dto = new()
        {
            Name = "New User",
            Email = "newuser@gmail.com",
            PhoneNumber = "+12184091047",
            Birthday = DateTime.UtcNow,
            Interest = "EDM",
            AgreeToSms = true
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/customers", dto, cancellationToken: TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
*/