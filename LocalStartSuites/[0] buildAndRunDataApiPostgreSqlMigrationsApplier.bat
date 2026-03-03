@echo off
cd ..
docker build src -f src/DataApi/AntiClown.Data.Api.PostgreSqlMigrationsApplier/Dockerfile -t localhost:5051/anticlowndataapipostgresql
docker push localhost:5051/anticlowndataapipostgresql
kubectl delete job anticlown-data-api-postgresql-job
kubectl apply -f k8s/anticlown-data-api-postgresql-job.yaml
pause
