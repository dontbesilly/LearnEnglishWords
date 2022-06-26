using System.Reflection;
using LearnEnglishWords.Repositories;
using LearnEnglishWords.WebApi;
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