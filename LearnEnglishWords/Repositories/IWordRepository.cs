using LearnEnglishWords.Models;

namespace LearnEnglishWords.Repositories;

public interface IWordRepository
{
    Task CreateWord(Word word);
    Task<Word> GetWordById(string id);
    Task<IEnumerable<Word>> GetAllWords();
    Task<Word> GetRandomWord();
}