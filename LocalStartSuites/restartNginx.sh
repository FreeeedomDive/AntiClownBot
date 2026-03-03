#!/bin/bash
cd ..
docker build nginx -f nginx/DockerfileLocal -t localhost:5051/anticlownnginx
docker push localhost:5051/anticlownnginx
kubectl rollout restart deployment/anticlown-nginx-deployment
