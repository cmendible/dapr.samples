docker build -t cmendibl3/dapr.sensors.actors .
docker push cmendibl3/dapr.sensors.actors

dapr run -a sensors -p 5000 -H 3500 -- dotnet run