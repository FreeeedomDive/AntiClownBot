@echo off
cd ..
docker build . -f Dockerfile.AntiClown.Data.Api.PostgreSqlMigrationsApplier -t localhost:5000/anticlowndataapipostgresql
docker push localhost:5000/anticlowndataapipostgresql
kubectl delete job anticlown-data-api-postgresql-job
kubectl apply -f k8s/anticlown-data-api-postgresql-job.yaml
pause