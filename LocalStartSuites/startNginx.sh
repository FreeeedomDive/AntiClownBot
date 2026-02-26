cd ..
docker build . -f Dockerfile.AntiClown.Nginx -t localhost:5051/anticlownnginx
docker push localhost:5051/anticlownnginx
cd k8s
kubectl apply -f anticlown-nginx-deployment.yaml