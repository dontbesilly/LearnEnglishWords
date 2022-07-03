using LearnEnglishWords.Exceptions;
using LearnEnglishWords.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LearnEnglishWords.Repositories;

public class RedisWordRepository : IWordRepository
{
    private const string HashSetName = "hashwords";

    private readonly IDatabase database;
    private readonly Random random;

    public RedisWordRepository(IConnectionMultiplexer redis)
    {
        database = redis.GetDatabase();
        random = new Random();
    }

    public async Task CreateWord(Word word)
    {
        if (word == null)
            throw new ArgumentOutOfRangeException(nameof(word));

        var hashEntries = await database.HashGetAllAsync(HashSetName);
        var words = Array.ConvertAll(hashEntries, val =>
            JsonConvert.DeserializeAnonymousType(val.Value, new {Name = ""})).ToList();

        if (words.FirstOrDefault(x => x.Name == word.Name) != null)
        {
            throw new AlreadyCreatedException("Already created");
        }

        await database.HashSetAsync(HashSetName, new HashEntry[]
        {
            new(word.Id, JsonSerializer.Serialize(word))
        });
    }

    public async Task<Word> GetWordById(string id)
    {
        var redisValue = await database.HashGetAsync(HashSetName, id);

        if (string.IsNullOrEmpty(redisValue))
        {
            throw new NotFoundException("Not found");
        }

        return JsonSerializer.Deserialize<Word>(redisValue);
    }

    public async Task<IEnumerable<Word>> GetAllWords()
    {
        var hashEntries = await database.HashGetAllAsync(HashSetName);

        if (hashEntries.Length <= 0)
            return new List<Word>();

        var words = Array.ConvertAll(hashEntries, val =>
            JsonSerializer.Deserialize<Word>(val.Value)).ToList();

        return words;
    }

    public async Task<Word> GetRandomWord()
    {
        var keys = await database.HashKeysAsync(HashSetName);

        var randomKey = random.Next(0, keys.Length);

        var redisValue = await database.HashGetAsync(HashSetName, keys[randomKey]);

        return JsonSerializer.Deserialize<Word>(redisValue);
    }

    public async Task EditWord(string id, Word word)
    {
        var redisValue = await database.HashGetAsync(HashSetName, id);

        if (string.IsNullOrEmpty(redisValue))
        {
            throw new NotFoundException("Not found");
        }

        await database.HashSetAsync(HashSetName, new HashEntry[]
        {
            new(word.Id, JsonSerializer.Serialize(word))
        });
    }

    public async Task DeleteWord(string id)
    {
        var word = await database.HashGetAsync(HashSetName, id);

        if (string.IsNullOrEmpty(word))
        {
            throw new NotFoundException("Not found");
        }

        await database.HashDeleteAsync(HashSetName, id, CommandFlags.FireAndForget);
    }
}