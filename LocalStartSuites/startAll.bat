@echo off
cd ../k8s
kubectl apply -f anticlown-api-postgresql-job.yaml
kubectl apply -f anticlown-entertainment-api-postgresql-job.yaml
kubectl apply -f anticlown-api-deployment.yaml
kubectl apply -f anticlown-entertainment-api-deployment.yaml
kubectl apply -f anticlown-events-daemon-deployment.yaml
kubectl apply -f anticlown-discord-bot-deployment.yaml
kubectl apply -f anticlown-nginx-deployment.yaml
pause