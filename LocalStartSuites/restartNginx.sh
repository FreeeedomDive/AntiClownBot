cd ..
docker build . -f Dockerfile.AntiClown.Nginx -t localhost:5051/anticlownnginx
docker push localhost:5051/anticlownnginx
kubectl rollout restart deployment/anticlown-nginx-deployment