@echo off
cd ..
docker build . -f Dockerfile.AntiClown.DiscordBot -t localhost:5000/anticlowndiscordbot
docker push localhost:5000/anticlowndiscordbot
kubectl rollout restart deployment/anticlown-discord-bot-deployment
pause