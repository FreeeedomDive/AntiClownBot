FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
COPY ["AntiClown.Api.PostgreSqlMigrationsApplier/AntiClown.Api.PostgreSqlMigrationsApplier.csproj", "AntiClown.Api.PostgreSqlMigrationsApplier/"]
RUN dotnet restore "AntiClown.Api.PostgreSqlMigrationsApplier/AntiClown.Api.PostgreSqlMigrationsApplier.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/AntiClown.Api.PostgreSqlMigrationsApplier"
RUN dotnet publish "AntiClown.Api.PostgreSqlMigrationsApplier.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AntiClown.Api.PostgreSqlMigrationsApplier.dll"]
