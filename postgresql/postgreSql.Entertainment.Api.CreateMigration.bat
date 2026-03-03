@echo off
set ASPNETCORE_ENVIRONMENT=Production
set /p "name=Enter migration name: "
dotnet tool install --global dotnet-ef
cd ..\src\EntertainmentApi
dotnet ef migrations add %name% --project AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier
pause