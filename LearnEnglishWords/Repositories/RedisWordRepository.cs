using System.Text.Json;
using LearnEnglishWords.Models;
using StackExchange.Redis;

namespace LearnEnglishWords.Repositories;

public class RedisWordRepository : IWordRepository
{
    private const string HashSetName = "hashwords";

    private readonly IConnectionMultiplexer redis;
    private readonly Random random;

    public RedisWordRepository(IConnectionMultiplexer redis)
    {
        this.redis = redis;
        random = new Random();
    }

    public async Task CreateWord(Word word)
    {
        if (word == null)
            throw new ArgumentOutOfRangeException(nameof(word));

        var db = redis.GetDatabase();
        var serialWord = JsonSerializer.Serialize(word);

        await db.HashSetAsync(HashSetName, new HashEntry[]
        {
            new(word.Id, serialWord)
        });
    }

    public async Task<Word> GetWordById(string id)
    {
        var db = redis.GetDatabase();

        var word = await db.HashGetAsync(HashSetName, id);

        if (string.IsNullOrEmpty(word))
            return null; // TODO

        return JsonSerializer.Deserialize<Word>(word);
    }

    public async Task<IEnumerable<Word>> GetAllWords()
    {
        var db = redis.GetDatabase();

        var hashEntries = await db.HashGetAllAsync(HashSetName);

        if (hashEntries.Length <= 0)
            return new List<Word>();

        var words = Array.ConvertAll(hashEntries, val =>
            JsonSerializer.Deserialize<Word>(val.Value)).ToList();

        return words;
    }

    public async Task<Word> GetRandomWord()
    {
        var db = redis.GetDatabase();

        var keys = await db.HashKeysAsync(HashSetName);

        var randomKey = random.Next(0, keys.Length);
        var value = keys[randomKey];

        var word = await db.HashGetAsync(HashSetName, value);

        if (string.IsNullOrEmpty(word))
            return null; // TODO

        return JsonSerializer.Deserialize<Word>(word);
    }
}