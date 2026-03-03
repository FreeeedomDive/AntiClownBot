@echo off
cd ..
docker build src -f src/AntiClown.EventsDaemon/Dockerfile -t localhost:5051/anticlowneventsdaemon
docker push localhost:5051/anticlowneventsdaemon
kubectl rollout restart deployment/anticlown-events-daemon-deployment
pause
