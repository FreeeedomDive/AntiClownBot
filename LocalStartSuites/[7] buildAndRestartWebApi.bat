@echo off
cd ..
docker build . -f Dockerfile.AntiClown.Web.Api -t localhost:5000/anticlownwebapi
docker push localhost:5000/anticlownwebapi
kubectl rollout restart deployment/anticlown-web-api-deployment
pause