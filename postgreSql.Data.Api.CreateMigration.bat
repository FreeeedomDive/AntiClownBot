@echo off
set ASPNETCORE_ENVIRONMENT=Production
set /p "name=Enter migration name: "
dotnet tool install --global dotnet-ef
dotnet ef migrations add %name% --project AntiClown.Data.Api.Core --startup-project AntiClown.Data.Api.PostgreSqlMigrationsApplier
pause