@echo off
cd ..
docker build . -f Dockerfile.AntiClown.DiscordBot.PostgreSqlMigrationsApplier -t localhost:5000/anticlowndiscordbotpostgresql
docker push localhost:5000/anticlowndiscordbotpostgresql
kubectl apply -f k8s/anticlown-discord-bot-postgresql-job.yaml
pause