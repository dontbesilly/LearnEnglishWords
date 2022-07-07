using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LearnEnglishWords.IntegrationTests.Fixture;

[ExcludeFromCodeCoverage]
public class TestServerFixture
{
    public TestServerFixture()
    {
        var application = new WebApplicationFactory<Program>();

        HttpClient = application.CreateClient();
    }

    public HttpClient HttpClient { get; }

    public JsonSerializerOptions JsonOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = {new JsonStringEnumConverter()}
    };
}