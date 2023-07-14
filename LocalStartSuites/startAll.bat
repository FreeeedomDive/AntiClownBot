@echo off
cd ../k8s
kubectl apply -f new-anticlown-api-deployment.yaml
kubectl apply -f new-anticlown-entertainment-api-deployment.yaml
kubectl apply -f new-anticlown-api-deployment.yaml
kubectl apply -f new-anticlown-discord-bot-deployment.yaml
pause