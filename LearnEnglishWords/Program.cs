using System.Net.Mime;
using System.Reflection;
using LearnEnglishWords.Exceptions;
using LearnEnglishWords.Repositories;
using LearnEnglishWords.WebApi;
using Microsoft.AspNetCore.Diagnostics;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder.Services);

var webApplication = builder.Build();

Configure(webApplication);

RegisterApi();

webApplication.Run();

void RegisterServices(IServiceCollection services)
{
    services.AddSingleton<IConnectionMultiplexer>(_ =>
        ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));

    services.AddTransient<IWordRepository, RedisWordRepository>();

    services.AddCors(opt => opt.AddPolicy("CorsPolicy",
        c => { c.WithOrigins("http://localhost:3000"); }));

    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddTransient<IApi, WordApi>();

    services.AddAutoMapper(Assembly.GetExecutingAssembly());
}

void Configure(IApplicationBuilder app)
{
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.ContentType = MediaTypeNames.Text.Plain;

            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

            context.Response.StatusCode = exceptionHandlerPathFeature?.Error switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                AlreadyCreatedException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            if (exceptionHandlerPathFeature?.Error != null)
                await context.Response.WriteAsync(exceptionHandlerPathFeature.Error.Message);
            else
                await context.Response.WriteAsync("Something wrong");
        });
    });

    app.UseHttpsRedirection();

    app.UseCors("CorsPolicy");

    app.UseSwagger();
    app.UseSwaggerUI();
}

void RegisterApi()
{
    var apis = webApplication.Services.GetServices<IApi>();
    foreach (var api in apis)
    {
        if (api is null)
            throw new InvalidProgramException("Api not found");

        api.Register(webApplication);
    }
}