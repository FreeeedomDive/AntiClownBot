FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
COPY ["AntiClown.DiscordBot.PostgreSqlMigrationsApplier/AntiClown.DiscordBot.PostgreSqlMigrationsApplier.csproj", "AntiClown.DiscordBot.PostgreSqlMigrationsApplier/"]
RUN dotnet restore "AntiClown.DiscordBot.PostgreSqlMigrationsApplier/AntiClown.DiscordBot.PostgreSqlMigrationsApplier.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/AntiClown.DiscordBot.PostgreSqlMigrationsApplier"
RUN dotnet publish "AntiClown.DiscordBot.PostgreSqlMigrationsApplier.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AntiClown.DiscordBot.PostgreSqlMigrationsApplier.dll"]
