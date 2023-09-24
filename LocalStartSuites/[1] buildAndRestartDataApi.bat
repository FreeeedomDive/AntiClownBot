@echo off
cd ..
docker build . -f Dockerfile.AntiClown.Data.Api -t localhost:5000/anticlowndataapi
docker push localhost:5000/anticlowndataapi
kubectl rollout restart deployment/anticlown-data-api-deployment
pause