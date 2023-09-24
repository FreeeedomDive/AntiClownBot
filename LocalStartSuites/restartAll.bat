@echo off
kubectl rollout restart deployment/anticlown-data-api-deployment
kubectl rollout restart deployment/anticlown-api-deployment
kubectl rollout restart deployment/anticlown-entertainment-api-deployment
kubectl rollout restart deployment/anticlown-events-daemon-deployment
kubectl rollout restart deployment/anticlown-discord-bot-deployment
kubectl rollout restart deployment/anticlown-nginx-deployment
pause