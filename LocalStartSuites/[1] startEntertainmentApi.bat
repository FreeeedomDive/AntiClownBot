@echo off
cd ..
docker build . -f Dockerfile.AntiClown.Entertainment.Api -t localhost:5000/anticlownentertainmentapi
docker push localhost:5000/anticlownentertainmentapi
kubectl rollout restart deployment/new-anticlown-entertainment-api-deployment
pause