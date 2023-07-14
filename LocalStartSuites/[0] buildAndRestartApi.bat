@echo off
cd ..
docker build . -f Dockerfile.AntiClown.Api -t localhost:5000/anticlownapi
docker push localhost:5000/anticlownapi
kubectl rollout restart deployment/new-anticlown-api-deployment
pause