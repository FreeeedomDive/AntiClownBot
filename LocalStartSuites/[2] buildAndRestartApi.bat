@echo off
cd ..
docker build src -f src/Api/AntiClown.Api/Dockerfile -t localhost:5000/anticlownapi
docker push localhost:5000/anticlownapi
kubectl rollout restart deployment/anticlown-api-deployment
pause
