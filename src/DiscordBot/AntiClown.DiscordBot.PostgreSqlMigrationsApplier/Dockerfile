FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
COPY ["DiscordBot/AntiClown.DiscordBot.PostgreSqlMigrationsApplier/AntiClown.DiscordBot.PostgreSqlMigrationsApplier.csproj", "DiscordBot/AntiClown.DiscordBot.PostgreSqlMigrationsApplier/"]
RUN dotnet restore "DiscordBot/AntiClown.DiscordBot.PostgreSqlMigrationsApplier/AntiClown.DiscordBot.PostgreSqlMigrationsApplier.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/DiscordBot/AntiClown.DiscordBot.PostgreSqlMigrationsApplier"
RUN dotnet publish "AntiClown.DiscordBot.PostgreSqlMigrationsApplier.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AntiClown.DiscordBot.PostgreSqlMigrationsApplier.dll"]
