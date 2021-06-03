```shell
docker build -t cmendibl3/dapr-twitter-producer .
docker push cmendibl3/dapr-twitter-producer

dapr run -a twitter-producer -p 5000 --components-path ..\components\ -- dotnet run
```