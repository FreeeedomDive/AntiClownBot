FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
COPY ["AntiClown.EventsDaemon/AntiClown.EventsDaemon.csproj", "AntiClown.EventsDaemon/"]
RUN dotnet restore "AntiClown.EventsDaemon/AntiClown.EventsDaemon.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/AntiClown.EventsDaemon"
RUN dotnet publish "AntiClown.EventsDaemon.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AntiClown.EventsDaemon.dll"]
