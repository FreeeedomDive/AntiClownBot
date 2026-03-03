@echo off
cd ..
docker build src -f src/DiscordBot/AntiClown.DiscordBot.PostgreSqlMigrationsApplier/Dockerfile -t localhost:5000/anticlowndiscordbotpostgresql
docker push localhost:5000/anticlowndiscordbotpostgresql
kubectl delete job anticlown-discord-bot-postgresql-job
kubectl apply -f k8s/anticlown-discord-bot-postgresql-job.yaml
pause
