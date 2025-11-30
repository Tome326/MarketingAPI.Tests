using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MarketingAPI;
using MarketingAPI.Models.DTOs;
using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Mvc.Testing;

namespace MarketingAPI.Test;

[Collection("API Tests")]
public class CustomerAPITest// : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly string _jwtToken;

    public CustomerAPITest(AuthenticationFixture fixture)
    {
        _client = fixture.Client;
        _jwtToken = fixture.JwtToken;
    }

    [Fact]
    public async Task GetCustomers_WithAuth_ReturnsSuccess()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        HttpResponseMessage response = await _client.GetAsync("/api/customers", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
        List<CustomerDto>? customerDtos = await response.Content.ReadFromJsonAsync<List<CustomerDto>>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(customerDtos);
        Assert.Contains(customerDtos, c => c.Name == "Fox Mulder");
        Assert.Contains(customerDtos, c => c.Name == "Dana Scully");
        Assert.Contains(customerDtos, c => c.Name == "Namey McNameFace");
    }

    [Fact]
    public async Task GetCustomerById_WithAuth_ReturnsSuccess()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        HttpResponseMessage response = await _client.GetAsync("/api/customers/1", TestContext.Current.CancellationToken);

        CustomerDto? dto = await response.Content.ReadFromJsonAsync<CustomerDto>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(dto);
        Assert.Equal("Namey McNameFace", dto.Name);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetCustomerByEmail_WithAuth_ReturnsSuccess()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        HttpResponseMessage response = await _client.GetAsync("/api/customers/by_email/McNameFace@email.com", TestContext.Current.CancellationToken);

        CustomerDto? dto = await response.Content.ReadFromJsonAsync<CustomerDto>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(dto);
        Assert.Equal("Namey McNameFace", dto.Name);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task DeleteCustomerById_WithAuth_ReturnsSuccess()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        HttpResponseMessage response = await _client.DeleteAsync("/api/customers/2", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task DeleteCustomerByEmail_WithAuth_ReturnsSuccess()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        HttpResponseMessage response = await _client.DeleteAsync("/api/customers/by_email/Scully@supersecretgovernmentthing.com", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AddCustomer_ReturnSuccess()
    {
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
    }

    [Fact]
    public async Task AddCustomer_DuplicateEmail_ReturnBadRequest()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        CustomerDto dto = new()
        {
            Name = "New User",
            Email = "McNameFace@email.com",
            PhoneNumber = "+12184091049",
            Birthday = DateTime.UtcNow,
            Interest = "EDM",
            AgreeToSms = true
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/customers", dto, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string? errContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("email already registered", errContent.ToLower());
    }

    [Fact]
    public async Task AddCustomer_DuplicatePhoneNumber_ReturnBadRequest()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        CustomerDto dto = new()
        {
            Name = "New User",
            Email = "newuser2@gmail.com",
            PhoneNumber = "+18777804236",
            Birthday = DateTime.UtcNow,
            Interest = "EDM",
            AgreeToSms = true
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/customers", dto, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        string? errContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("phone number already registered", errContent.ToLower());
    }

    [Fact]
    public async Task GetCustomerMetrics_WithAuth_ReturnsSuccess()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        HttpResponseMessage response = await _client.GetAsync("/api/customers/metrics", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        CustomerMetricsDto? dto = await response.Content.ReadFromJsonAsync<CustomerMetricsDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(dto);
    }

    [Fact]
    public async Task GetCustomerInterests_WithAuth_ReturnsSuccess()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        HttpResponseMessage response = await _client.GetAsync("/api/customers/interests", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        List<string>? interests = await response.Content.ReadFromJsonAsync<List<string>>(TestContext.Current.CancellationToken);
        Assert.NotNull(interests);
    }
}

/*
        Customer customer = new()
        {
            Name = customerDto.Name,
            Email = customerDto.Email,
            PhoneNumber = formattedPhoneNumber,
            Birthday = customerDto.Birthday,
            Interest = customerDto.Interest,
            AgreeToSms = customerDto.AgreeToSms
        };
*/