@echo off
cd ..
docker build src -f src/Web/AntiClown.Web.Api/Dockerfile -t localhost:5000/anticlownwebapi
docker push localhost:5000/anticlownwebapi
kubectl rollout restart deployment/anticlown-web-api-deployment
pause
