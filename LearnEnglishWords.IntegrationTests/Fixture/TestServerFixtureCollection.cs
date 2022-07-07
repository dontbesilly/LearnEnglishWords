using System.Diagnostics.CodeAnalysis;

namespace LearnEnglishWords.IntegrationTests.Fixture;

[ExcludeFromCodeCoverage]
[CollectionDefinition("TestServer collection")]
public class TestServerFixtureCollection : ICollectionFixture<TestServerFixture>
{
}