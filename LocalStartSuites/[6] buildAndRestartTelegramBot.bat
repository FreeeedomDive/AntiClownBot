@echo off
cd ..
docker build src -f src/AntiClown.Telegram.Bot/Dockerfile -t localhost:5000/anticlowntelegrambot
docker push localhost:5000/anticlowntelegrambot
kubectl rollout restart deployment/anticlown-telegram-bot-deployment
pause
