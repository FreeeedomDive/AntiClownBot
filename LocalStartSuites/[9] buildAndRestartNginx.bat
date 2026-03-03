@echo off
cd ..
docker build nginx -f nginx/Dockerfile -t localhost:5000/anticlownnginx
docker push localhost:5000/anticlownnginx
kubectl rollout restart deployment/anticlown-nginx-deployment
pause
