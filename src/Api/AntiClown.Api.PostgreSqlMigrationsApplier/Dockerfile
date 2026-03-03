FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
COPY ["Api/AntiClown.Api.PostgreSqlMigrationsApplier/AntiClown.Api.PostgreSqlMigrationsApplier.csproj", "Api/AntiClown.Api.PostgreSqlMigrationsApplier/"]
RUN dotnet restore "Api/AntiClown.Api.PostgreSqlMigrationsApplier/AntiClown.Api.PostgreSqlMigrationsApplier.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/Api/AntiClown.Api.PostgreSqlMigrationsApplier"
RUN dotnet publish "AntiClown.Api.PostgreSqlMigrationsApplier.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AntiClown.Api.PostgreSqlMigrationsApplier.dll"]
