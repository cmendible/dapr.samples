FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ./dapr.sensors.interfaces/dapr.sensors.interfaces.csproj ./dapr.sensors.interfaces/dapr.sensors.interfaces.csproj
RUN dotnet restore ./dapr.sensors.interfaces/dapr.sensors.interfaces.csproj

# Copy csproj and restore as distinct layers
COPY ./dapr.sensors.client/dapr.sensors.client.csproj ./dapr.sensors.client/dapr.sensors.client.csproj
RUN dotnet restore ./dapr.sensors.client/dapr.sensors.client.csproj

# Copy everything else and build website
COPY ../dapr.sensors.interfaces/ ./dapr.sensors.interfaces/
COPY ./dapr.sensors.client/ ./dapr.sensors.client/
WORKDIR /src/dapr.sensors.client/
RUN dotnet publish -c release

# Final stage / image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY --from=build /src/dapr.sensors.client/bin/release/net5.0/publish ./
COPY ./dapr.sensors.client/start.sh ./start.sh

ENTRYPOINT ["./start.sh"]