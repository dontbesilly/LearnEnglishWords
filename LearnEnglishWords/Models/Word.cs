namespace LearnEnglishWords.Models;

public class Word
{
    public string Id { get; init; } = $"word:{Guid.NewGuid()}";
    public string Name { get; init; }
    public string Translate { get; init; }
}