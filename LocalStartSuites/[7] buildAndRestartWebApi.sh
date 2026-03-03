#!/bin/bash
cd ..
docker build src -f src/Web/AntiClown.Web.Api/Dockerfile -t localhost:5051/anticlownwebapi
docker push localhost:5051/anticlownwebapi
kubectl rollout restart deployment/anticlown-web-api-deployment
