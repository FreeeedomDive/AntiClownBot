@echo off
set ASPNETCORE_ENVIRONMENT=Production
set /p "name=Enter migration name: "
dotnet tool install --global dotnet-ef
dotnet ef migrations add %name% --project AntiClown.DiscordBot --startup-project AntiClown.DiscordBot.PostgreSqlMigrationsApplier
pause