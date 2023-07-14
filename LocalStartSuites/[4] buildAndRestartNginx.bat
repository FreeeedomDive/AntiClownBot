@echo off
cd ..
docker build . -f Dockerfile.AntiClown.Nginx -t localhost:5000/anticlownnginx
docker push localhost:5000/anticlownnginx
kubectl rollout restart deployment/new-anticlown-nginx-deployment
pause