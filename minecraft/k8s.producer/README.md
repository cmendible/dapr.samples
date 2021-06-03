```shell
docker build -t cmendibl3/dapr-k8s-producer .
docker push cmendibl3/dapr-k8s-producer

dapr run -a k8s-producer -p 5000 --components-path ..\components\ -- dotnet run
```