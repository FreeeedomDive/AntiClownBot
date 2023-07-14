@echo off
cd ..
docker build . -f Dockerfile.AntiClown.Nginx -t localhost:5000/anticlownnginx
docker push localhost:5000/anticlownnginx
kubectl rollout restart deployment/anticlown-nginx-deployment
pause