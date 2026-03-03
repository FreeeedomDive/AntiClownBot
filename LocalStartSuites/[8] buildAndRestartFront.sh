#!/bin/bash
cd ../src/Web/AntiClown.Web.Front
docker build . -f Dockerfile -t localhost:5051/anticlownwebfront
docker push localhost:5051/anticlownwebfront
kubectl rollout restart deployment/anticlown-web-front-deployment
