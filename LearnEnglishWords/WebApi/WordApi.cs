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
        app.MapGet("/api/word", GetAll)
            .Produces<List<Word>>(StatusCodes.Status200OK)
            .WithName(nameof(GetAll))
            .WithTags("Word");

        app.MapGet("/api/word/{id}", GetWordById)
            .Produces<Word>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(nameof(GetWordById))
            .WithTags("Word");

        app.MapPost("/api/word", CreateWord)
            .Produces<Word>(StatusCodes.Status201Created)
            .WithName(nameof(CreateWord))
            .WithTags("Word");

        app.MapPut("/api/word/{id}", EditWord)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(nameof(EditWord))
            .WithTags("Word");

        app.MapDelete("/api/word/{id}", DeleteWord)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(nameof(DeleteWord))
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

    private async Task<IResult> GetWordById([FromRoute] string id)
    {
        var word = await wordRepository.GetWordById(id);

        return Results.Ok(word);
    }

    private async Task<IResult> CreateWord([FromBody] WordDto dto, [FromServices] IMapper mapper)
    {
        var word = mapper.Map<Word>(dto);

        await wordRepository.CreateWord(word);

        return Results.Created(nameof(CreateWord), new {word.Id});
    }

    private async Task<IResult> EditWord([FromRoute] string id, [FromBody] WordDto dto, [FromServices] IMapper mapper)
    {
        var word = mapper.Map<Word>(dto);
        word.Id = id;

        await wordRepository.EditWord(id, word);

        return Results.Ok();
    }

    private async Task<IResult> DeleteWord([FromRoute] string id)
    {
        await wordRepository.DeleteWord(id);

        return Results.Ok();
    }
}