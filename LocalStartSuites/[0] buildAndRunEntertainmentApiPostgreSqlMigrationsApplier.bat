@echo off
cd ..
docker build src -f src/AntiClown.Entertainment.Api.PostgreSqlMigrationsApplier/Dockerfile -t localhost:5000/anticlownentertainmentapipostgresql
docker push localhost:5000/anticlownentertainmentapipostgresql
kubectl delete job anticlown-entertainment-api-postgresql-job
kubectl apply -f k8s/anticlown-entertainment-api-postgresql-job.yaml
pause
