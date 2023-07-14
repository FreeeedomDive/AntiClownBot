@echo off
kubectl delete deploy new-anticlown-api-deployment
kubectl delete deploy new-anticlown-entertainment-api-deployment
kubectl delete deploy new-anticlown-events-daemon-deployment
kubectl delete deploy new-anticlown-discord-bot-deployment
pause