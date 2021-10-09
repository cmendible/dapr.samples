docker build -t cmendibl3/dapr.sensors.actors -f .\dapr.actors.Dockerfile .
docker build -t cmendibl3/dapr.sensors.client -f .\dapr.client.Dockerfile .
docker build -t cmendibl3/dapr.sensors.average -f .\dapr.sensors.average.Dockerfile .

docker push cmendibl3/dapr.sensors.actors
docker push cmendibl3/dapr.sensors.client
docker push cmendibl3/dapr.sensors.average

terraform apply

az aks get-credentials -n dapr-aks -g dapr-demo --overwrite-existing

dapr init -k

kubectl create deployment zipkin --image openzipkin/zipkin
kubectl expose deployment zipkin --type ClusterIP --port 9411
kubectl apply -f ./deploy/config/tracing.yaml

kubectl apply -f ./sensors.deploy.yaml


---

nova give WIND_TURBINE
