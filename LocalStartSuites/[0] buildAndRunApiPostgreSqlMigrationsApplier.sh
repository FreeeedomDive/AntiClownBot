#!/bin/bash
cd ..
docker build src -f src/AntiClown.Api.PostgreSqlMigrationsApplier/Dockerfile -t localhost:5000/anticlownapipostgresql
docker push localhost:5000/anticlownapipostgresql
kubectl delete job anticlown-api-postgresql-job
kubectl apply -f k8s/anticlown-api-postgresql-job.yaml
