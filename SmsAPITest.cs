using System.Net.Http.Headers;
using System.Net.Http.Json;
using MarketingAPI.Models.DTOs;

namespace MarketingAPI.Test;

[Collection("API Tests")]
public class SmsAPITest
{
    private readonly HttpClient _client;
    private readonly string _jwtToken;

    public SmsAPITest(AuthenticationFixture fixture)
    {
        _client = fixture.Client;
        _jwtToken = fixture.JwtToken;
    }

    [Fact]
    public async Task SendSms_WithAuth_ReturnsSuccess()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        SmsDto dto = new()
        {
            Recipient = "+18777804236",
            Message = "Test Message Sending"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/sms/send", dto, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task SendBulkSms_WithAuth_ReturnsSuccess()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        BulkSmsDto dto = new()
        {
            MessageTemplate = "Hello, {name}, this is a test bulk message!",
            RecipientTag = "event:EDM"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/sms/bulk", dto, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }
}