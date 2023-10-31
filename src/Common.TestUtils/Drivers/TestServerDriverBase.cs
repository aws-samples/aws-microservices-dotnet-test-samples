using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace Common.TestUtils.Drivers;

public class TestServerDriverBase<TProgram> : IDisposable where TProgram : class
{
    private readonly WebApplicationFactory<TProgram> _application;
    private readonly JsonSerializerOptions _options;

    protected TestServerDriverBase(params (string key, string value)[] settings)
    {
        _application = new WebApplicationFactory<TProgram>()
            .WithWebHostBuilder(builder =>
            {
                foreach (var (key, value) in settings) builder.UseSetting(key, value);
            });


        Client = _application.CreateClient();

        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    protected HttpClient Client { get; }

    public void Dispose()
    {
        _application.Dispose();
        Client.Dispose();
    }

    protected async Task<T> GetResultFromResponse<T>(HttpResponseMessage response)
    {
        VerifyResponse(response);

        var responseJson = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<T>(responseJson, _options);

        Assert.That(result, Is.Not.Null);

        return result!;
    }

    protected static void VerifyResponse(HttpResponseMessage response)
    {
        Assert.True(response.IsSuccessStatusCode, $"got: {response.StatusCode}");
    }
}