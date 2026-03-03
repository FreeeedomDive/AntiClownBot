@echo off
cd ../src/Web/AntiClown.Web.Front
docker build . -f Dockerfile -t localhost:5000/anticlownwebfront
docker push localhost:5000/anticlownwebfront
kubectl rollout restart deployment/anticlown-web-front-deployment
pause
