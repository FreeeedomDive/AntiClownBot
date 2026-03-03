#!/bin/bash
cd ..
docker build src -f src/Api/AntiClown.Api/Dockerfile -t localhost:5051/anticlownapi
docker push localhost:5051/anticlownapi
kubectl rollout restart deployment/anticlown-api-deployment
