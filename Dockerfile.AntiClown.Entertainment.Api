FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /certificate
RUN dotnet dev-certs https --clean
RUN mkdir https
RUN dotnet dev-certs https -ep ./https/certificate.pfx -p aspnet
RUN dotnet dev-certs https --trust

WORKDIR /src
COPY ["AntiClown.Entertainment.Api/AntiClown.Entertainment.Api.csproj", "AntiClown.Entertainment.Api/"]
RUN dotnet restore "AntiClown.Entertainment.Api/AntiClown.Entertainment.Api.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/AntiClown.Entertainment.Api"
RUN dotnet publish "AntiClown.Entertainment.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /certificate /certificate
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AntiClown.Entertainment.Api.dll"]
