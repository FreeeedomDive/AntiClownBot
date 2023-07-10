@echo off
set ASPNETCORE_ENVIRONMENT=Production
dotnet tool install --global dotnet-ef
dotnet ef database update --project AntiClown.Entertainment.Api.Core --startup-project AntiClown.Entertainment.Api
pause