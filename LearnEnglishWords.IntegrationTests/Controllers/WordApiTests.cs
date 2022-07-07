using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using AutoFixture.Xunit2;
using LearnEnglishWords.IntegrationTests.Fixture;
using LearnEnglishWords.Models;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace LearnEnglishWords.IntegrationTests.Controllers;

[ExcludeFromCodeCoverage]
[Collection("TestServer collection")]
public class WordApiTests
{
    private readonly ITestOutputHelper testOutputHelper;
    private readonly HttpClient httpClient;

    public WordApiTests(TestServerFixture testServerFixture, ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
        httpClient = testServerFixture.HttpClient;
    }

    [Theory]
    [AutoData]
    public async Task GetAllWords_StatusOk(Word word)
    {
        // Arrange
        await httpClient.PostAsJsonAsync("api/word", word);

        // Act
        var httpResponseMessage = await httpClient.GetAsync("api/word");

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        testOutputHelper.WriteLine(await httpResponseMessage.Content.ReadAsStringAsync());
    }

    [Theory]
    [AutoData]
    public async Task GetRandomWord_StatusOk(Word word)
    {
        // Arrange
        await httpClient.PostAsJsonAsync("api/word", word);

        // Act
        var httpResponseMessage = await httpClient.GetAsync("api/word/random");

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        testOutputHelper.WriteLine(await httpResponseMessage.Content.ReadAsStringAsync());
    }

    [Theory]
    [AutoData]
    public async Task GetWordById_StatusOk(Word word)
    {
        // Arrange
        var response = await httpClient.PostAsJsonAsync("api/word", word);
        word.Id = await WordId(response);

        // Act
        var httpResponseMessage = await httpClient.GetAsync($"api/word/{word.Id}");

        // Assert
        var result = await httpResponseMessage.Content.ReadFromJsonAsync<Word>();
        result!.Id.Should().Be(word.Id);
    }

    [Theory]
    [AutoData]
    public async Task CreateWord_StatusCreated(Word word)
    {
        // Arrange

        // Act
        var httpResponseMessage = await httpClient.PostAsJsonAsync("api/word", word);

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Theory]
    [AutoData]
    public async Task UpdateWord_StatusOk(Word word)
    {
        // Arrange
        var response = await httpClient.PostAsJsonAsync("api/word", word);
        word.Id = await WordId(response);
        word.Name = Guid.NewGuid().ToString();

        // Act
        var httpResponseMessage = await httpClient.PutAsJsonAsync($"api/word/{word.Id}", word);

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponseMessage = await httpClient.GetAsync($"api/word/{word.Id}");
        var result = await getResponseMessage.Content.ReadFromJsonAsync<Word>();
        result!.Name.Should().Be(word.Name);
    }

    [Theory]
    [AutoData]
    public async Task DeleteWord_StatusOk(Word word)
    {
        // Arrange
        var response = await httpClient.PostAsJsonAsync("api/word", word);
        word.Id = await WordId(response);

        // Act
        var httpResponseMessage = await httpClient.DeleteAsync($"api/word/{word.Id}");

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static async Task<string> WordId(HttpResponseMessage response)
    {
        var newWord = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), new {Id = ""});
        return newWord.Id;
    }
}