#!/bin/bash
cd ..
docker build src -f src/DiscordBot/AntiClown.DiscordBot/Dockerfile -t localhost:5051/anticlowndiscordbot
docker push localhost:5051/anticlowndiscordbot
kubectl rollout restart deployment/anticlown-discord-bot-deployment
