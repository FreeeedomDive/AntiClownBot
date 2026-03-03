#!/bin/bash
cd ..
docker build src -f src/AntiClown.Data.Api/Dockerfile -t localhost:5000/anticlowndataapi
docker push localhost:5000/anticlowndataapi
kubectl rollout restart deployment/anticlown-data-api-deployment
