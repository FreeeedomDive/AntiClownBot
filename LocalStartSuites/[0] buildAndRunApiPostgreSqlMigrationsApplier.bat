@echo off
cd ..
docker build . -f Dockerfile.AntiClown.Api.PostgreSqlMigrationsApplier -t localhost:5000/anticlownapipostgresql
docker push localhost:5000/anticlownapipostgresql
kubectl delete job anticlown-api-postgresql-job
kubectl apply -f k8s/anticlown-api-postgresql-job.yaml
pause