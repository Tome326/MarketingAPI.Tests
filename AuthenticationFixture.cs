using System.Net.Http.Json;
using MarketingAPI;
using MarketingAPI.Data;
using MarketingAPI.Models;
using MarketingAPI.Models.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MarketingAPI.Test;

public class AuthenticationFixture : IAsyncLifetime
{
    public HttpClient Client { get; private set; } = null!;
    public string JwtToken { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        SqliteConnection connection = new("DataSource=:memory:");
        connection.Open();

        WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    ServiceDescriptor? descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseSqlite(connection);
                    });
                });
            }); ;

        using (IServiceScope scope = factory.Services.CreateScope())
        {
            ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();
            await SeedTestData(db);
        }

        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:7010")
        });

        RegisterDto registerRequest = new()
        {
            Email = "test@email.com",
            Username = "test",
            Password = "test123!"
        };

        HttpResponseMessage registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        if (!registerResponse.IsSuccessStatusCode)
            throw new Exception("Failed to create user - all tests will fail");

        LoginDto loginRequest = new()
        {
            Username = "test",
            Password = "test123!"
        };

        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to authenticate - all tests will fail");

        LoginResponseDto? loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        JwtToken = loginResponse!.Token;
    }

    private async Task SeedTestData(ApplicationDbContext db)
    {
        db.Customers.AddRange(
            new Customer
            {
                Name = "Namey McNameFace",
                Email = "McNameFace@email.com",
                PhoneNumber = "+18777804236",
                Birthday = DateTime.UtcNow,
                Interest = "EDM",
                AgreeToSms = true
            },
            new Customer
            {
                Name = "Fox Mulder",
                Email = "Mulder@supersecretgovernmentthing.com",
                PhoneNumber = "+12188393625",
                Birthday = DateTime.UtcNow,
                Interest = "EDM",
                AgreeToSms = true
            },
            new Customer
            {
                Name = "Dana Scully",
                Email = "Scully@supersecretgovernmentthing.com",
                PhoneNumber = "+12188393626",
                Birthday = DateTime.UtcNow,
                Interest = "EDM",
                AgreeToSms = true
            }
        );

        await db.SaveChangesAsync();
    }

    public ValueTask DisposeAsync()
    {
        Client?.Dispose();
        return ValueTask.CompletedTask;
    }
}

/*
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Optional: Replace real database with in-memory for testing
                    // Remove real DbContext
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add in-memory database
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });

                builder.ConfigureAppConfiguration((context, config) =>
                {
                    // Optional: Add test configuration
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Jwt:Key"] = "your-test-secret-key-here",
                        ["Jwt:Issuer"] = "test-issuer"
                    }!);
                });
            });
*/