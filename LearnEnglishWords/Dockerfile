FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LearnEnglishWords/LearnEnglishWords.csproj", "LearnEnglishWords/"]
RUN dotnet restore "LearnEnglishWords/LearnEnglishWords.csproj"
COPY . .
WORKDIR "/src/LearnEnglishWords"
RUN dotnet build "LearnEnglishWords.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LearnEnglishWords.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LearnEnglishWords.dll"]
