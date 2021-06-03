docker build -t cmendibl3/dapr-twitter-sentiment .
docker push cmendibl3/dapr-twitter-sentiment 

dapr run -a twitter-sentiment -p 5000 --components-path ..\components\ -- dotnet run
