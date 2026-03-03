@echo off
cd ..
docker build src -f src/Api/AntiClown.Api.PostgreSqlMigrationsApplier/Dockerfile -t localhost:5051/anticlownapipostgresql
docker push localhost:5051/anticlownapipostgresql
kubectl delete job anticlown-api-postgresql-job
kubectl apply -f k8s/anticlown-api-postgresql-job.yaml
pause
