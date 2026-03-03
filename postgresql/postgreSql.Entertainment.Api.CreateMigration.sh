#!/bin/sh

export ASPNETCORE_ENVIRONMENT=Production

printf "Enter migration name: "
read name

dotnet tool install --global dotnet-ef

cd ../src/EntertainmentApi
dotnet ef migrations add "$name" \
  --project AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier
