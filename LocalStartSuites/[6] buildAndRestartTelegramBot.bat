@echo off
cd ..
docker build . -f Dockerfile.AntiClown.TelegramBot -t localhost:5000/anticlowntelegrambot
docker push localhost:5000/anticlowntelegrambot
kubectl rollout restart deployment/anticlown-telegram-bot-deployment
pause