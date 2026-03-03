@echo off
set ASPNETCORE_ENVIRONMENT=Production
set /p "name=Enter migration name: "
dotnet tool install --global dotnet-ef
cd ..\src\Api
dotnet ef migrations add %name% --project AntiClown.Api.PostgreSqlMigrationsApplier
pause