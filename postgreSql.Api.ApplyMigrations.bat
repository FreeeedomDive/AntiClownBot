@echo off
set ASPNETCORE_ENVIRONMENT=Production
dotnet tool install --global dotnet-ef
dotnet ef database update --project AntiClown.Api.Core --startup-project AntiClown.Api
pause