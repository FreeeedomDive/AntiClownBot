@echo off
cd ..
docker build src -f src/AntiClown.Telegram.Bot/Dockerfile -t localhost:5051/anticlowntelegrambot
docker push localhost:5051/anticlowntelegrambot
kubectl rollout restart deployment/anticlown-telegram-bot-deployment
pause
