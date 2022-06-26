using LearnEnglishWords.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

        var hashEntries = await db.HashGetAllAsync(HashSetName);
        var words = Array.ConvertAll(hashEntries, val =>
            JsonConvert.DeserializeAnonymousType(val.Value, new {Name = ""})).ToList();

        if (words.FirstOrDefault(x => x.Name == word.Name) != null)
        {
            // TODO
            throw new Exception("Already created");
        }

        await db.HashSetAsync(HashSetName, new HashEntry[]
        {
            new(word.Id, JsonSerializer.Serialize(word))
        });
    }

    public async Task<Word> GetWordById(string id)
    {
        var db = redis.GetDatabase();

        var word = await db.HashGetAsync(HashSetName, id);

        if (string.IsNullOrEmpty(word))
        {
            // TODO
            throw new Exception("Not found");
        }

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

        var redisValue = await db.HashGetAsync(HashSetName, keys[randomKey]);

        return JsonSerializer.Deserialize<Word>(redisValue);
    }
}