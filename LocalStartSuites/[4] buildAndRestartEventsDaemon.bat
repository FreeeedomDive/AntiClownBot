@echo off
cd ..
docker build . -f Dockerfile.AntiClown.EventsDaemon -t localhost:5000/anticlowneventsdaemon
docker push localhost:5000/anticlowneventsdaemon
kubectl rollout restart deployment/anticlown-events-daemon-deployment
pause