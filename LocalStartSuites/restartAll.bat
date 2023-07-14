@echo off
kubectl rollout restart deployment/new-anticlown-api-deployment
kubectl rollout restart deployment/new-anticlown-entertainment-api-deployment
kubectl rollout restart deployment/new-anticlown-events-daemon-deployment
kubectl rollout restart deployment/new-anticlown-discord-bot-deployment
kubectl rollout restart deployment/new-anticlown-nginx-deployment
pause