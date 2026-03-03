@echo off
cd ..
docker build src -f src/DataApi/AntiClown.Data.Api/Dockerfile -t localhost:5051/anticlowndataapi
docker push localhost:5051/anticlowndataapi
kubectl rollout restart deployment/anticlown-data-api-deployment
pause
