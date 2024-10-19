@echo off
cd ../AntiClown.Web.Front
docker build . -f Dockerfile.AntiClown.Web.Front -t localhost:5000/anticlownwebfront
docker push localhost:5000/anticlownwebfront
kubectl rollout restart deployment/anticlown-web-front-deployment
pause