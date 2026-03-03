@echo off
cd ..
docker build src -f src/EntertainmentApi/AntiClown.Entertainment.Api/Dockerfile -t localhost:5051/anticlownentertainmentapi
docker push localhost:5051/anticlownentertainmentapi
kubectl rollout restart deployment/anticlown-entertainment-api-deployment
pause
