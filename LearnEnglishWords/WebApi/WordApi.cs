using AutoMapper;
using LearnEnglishWords.Dto;
using LearnEnglishWords.Models;
using LearnEnglishWords.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LearnEnglishWords.WebApi;

public class WordApi : IApi
{
    private readonly IWordRepository wordRepository;

    public WordApi(IWordRepository wordRepository)
    {
        this.wordRepository = wordRepository;
    }

    public void Register(WebApplication app)
    {
        app.MapPost("/api/word", CreateWord)
            .Produces<Word>(StatusCodes.Status201Created)
            .WithName(nameof(CreateWord))
            .WithTags("Word");

        app.MapGet("/api/word/{id}", GetWordById)
            .Produces<Word>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(nameof(GetWordById))
            .WithTags("Word");

        app.MapGet("/api/word", GetAll)
            .Produces<List<Word>>(StatusCodes.Status200OK)
            .WithName(nameof(GetAll))
            .WithTags("Word");

        app.MapGet("/api/word/random", GetRandomWord)
            .Produces<Word>(StatusCodes.Status200OK)
            .WithName(nameof(GetRandomWord))
            .WithTags("Word");
    }

    private async Task<IResult> GetRandomWord()
    {
        var result = await wordRepository.GetRandomWord();

        return Results.Ok(result);
    }

    private async Task<IResult> GetAll()
    {
        var result = await wordRepository.GetAllWords();

        return Results.Ok(result);
    }

    private async Task<IResult> GetWordById([FromQuery] string id)
    {
        var word = await wordRepository.GetWordById(id);

        if (word is null)
            return await Task.FromResult(Results.NotFound());

        return Results.Ok(word);
    }

    private async Task<IResult> CreateWord([FromBody] WordDto dto, [FromServices] IMapper mapper)
    {
        var word = mapper.Map<Word>(dto);

        await wordRepository.CreateWord(word);
        return Results.Created(nameof(CreateWord), new {word.Id});
    }
}