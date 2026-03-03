#!/bin/bash
cd ..
docker build src -f src/EntertainmentApi/AntiClown.Entertainment.Api/Dockerfile -t localhost:5000/anticlownentertainmentapi
docker push localhost:5000/anticlownentertainmentapi
kubectl rollout restart deployment/anticlown-entertainment-api-deployment
