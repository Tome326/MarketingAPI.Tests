using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MarketingAPI.Models.DTOs;

namespace MarketingAPI.Test;

[Collection("API Tests")]
public class UserAPITest
{
    private readonly HttpClient _client;
    private readonly string _jwtToken;

    public UserAPITest(AuthenticationFixture fixture)
    {
        _client = fixture.Client;
        _jwtToken = fixture.JwtToken;
    }

    [Fact]
    public async Task Me_WithAuth_ReturnSuccess()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        HttpResponseMessage response = await _client.GetAsync("/api/users/me", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        UserDto? dto = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(dto);
        Assert.Equal("test", dto.Username);
    }

    [Fact]
    public async Task GetAllUsers_WithAuth_ReturnsSuccess()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        HttpResponseMessage response = await _client.GetAsync("/api/users", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetUser_WithAuth_ReturnsSuccess()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        HttpResponseMessage response = await _client.GetAsync("/api/users/1", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task DeleteUser_WithAuth_ReturnsSuccess()
    {
        RegisterDto registerDto = new()
        {
            Username = "testDelete",
            Password = "DeletePassword",
            Email = "deleteme@user.com"
        };

        HttpResponseMessage registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto, TestContext.Current.CancellationToken);
        registerResponse.EnsureSuccessStatusCode();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        HttpResponseMessage deleteResponse = await _client.DeleteAsync("api/users/2", TestContext.Current.CancellationToken);
        deleteResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Me_NoAuth_ReturnsUnauthorized()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        HttpResponseMessage response = await _client.GetAsync("/api/users/me", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllUsers_NoAuth_ReturnsUnauthorized()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        HttpResponseMessage response = await _client.GetAsync("/api/users", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUser_NoAuth_ReturnsUnauthorized()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        HttpResponseMessage response = await _client.GetAsync("/api/users/1", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_NoAuth_ReturnsUnauthorized()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        HttpResponseMessage response = await _client.GetAsync("/api/users/1", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}