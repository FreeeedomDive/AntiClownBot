@echo off
cd ..
docker build src -f src/DiscordBot/AntiClown.DiscordBot/Dockerfile -t localhost:5000/anticlowndiscordbot
docker push localhost:5000/anticlowndiscordbot
kubectl rollout restart deployment/anticlown-discord-bot-deployment
pause
