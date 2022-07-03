namespace LearnEnglishWords.Models;

public class Word
{
    public string Id { get; set; } = $"word:{Guid.NewGuid()}";
    public string Name { get; set; }
    public string Translate { get; set; }
}