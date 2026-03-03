FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
COPY ["DataApi/AntiClown.Data.Api.PostgreSqlMigrationsApplier/AntiClown.Data.Api.PostgreSqlMigrationsApplier.csproj", "DataApi/AntiClown.Data.Api.PostgreSqlMigrationsApplier/"]
RUN dotnet restore "DataApi/AntiClown.Data.Api.PostgreSqlMigrationsApplier/AntiClown.Data.Api.PostgreSqlMigrationsApplier.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/DataApi/AntiClown.Data.Api.PostgreSqlMigrationsApplier"
RUN dotnet publish "AntiClown.Data.Api.PostgreSqlMigrationsApplier.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AntiClown.Data.Api.PostgreSqlMigrationsApplier.dll"]
