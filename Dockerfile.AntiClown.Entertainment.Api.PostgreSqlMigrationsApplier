FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
COPY ["AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier/AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier.csproj", "AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier/"]
RUN dotnet restore "AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier/AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier"
RUN dotnet publish "AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier.dll"]