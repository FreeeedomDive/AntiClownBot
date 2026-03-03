FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

RUN apt-get update && apt-get install -y libopus-dev libopus0 libsodium23 libsodium-dev

WORKDIR /certificate
RUN dotnet dev-certs https --clean
RUN mkdir https
RUN dotnet dev-certs https -ep ./https/certificate.pfx -p aspnet
RUN dotnet dev-certs https --trust

WORKDIR /src
COPY ["AntiClown.DiscordBot/AntiClown.DiscordBot.csproj", "AntiClown.DiscordBot/"]
RUN dotnet restore "AntiClown.DiscordBot/AntiClown.DiscordBot.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/AntiClown.DiscordBot"
RUN dotnet publish "AntiClown.DiscordBot.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app
COPY --from=build /usr/lib/x86_64-linux-gnu/libopus.* .
COPY --from=build /usr/lib/x86_64-linux-gnu/libsodium.* .
COPY --from=build /certificate /certificate
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AntiClown.DiscordBot.dll"]
