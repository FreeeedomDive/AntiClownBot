@echo off
kubectl delete deploy anticlown-nginx-deployment
kubectl delete deploy anticlown-discord-bot-deployment
kubectl delete deploy anticlown-events-daemon-deployment
kubectl delete deploy anticlown-entertainment-api-deployment
kubectl delete deploy anticlown-api-deployment
kubectl delete deploy anticlown-data-api-deployment
kubectl delete job anticlown-discord-bot-postgresql-job
kubectl delete job anticlown-entertainment-api-postgresql-job
kubectl delete job anticlown-api-postgresql-job
kubectl delete job anticlown-data-api-postgresql-job
pause