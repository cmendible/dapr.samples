FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ./dapr.sensors.interfaces/dapr.sensors.interfaces.csproj ./dapr.sensors.interfaces/dapr.sensors.interfaces.csproj
RUN dotnet restore ./dapr.sensors.interfaces/dapr.sensors.interfaces.csproj

# Copy csproj and restore as distinct layers
COPY ./dapr.sensors.actors/dapr.sensors.actors.csproj ./dapr.sensors.actors/dapr.sensors.actors.csproj
RUN dotnet restore ./dapr.sensors.actors/dapr.sensors.actors.csproj

# Copy everything else and build website
COPY ../dapr.sensors.interfaces/ ./dapr.sensors.interfaces/
COPY ./dapr.sensors.actors/ ./dapr.sensors.actors/
WORKDIR /src/dapr.sensors.actors/
RUN dotnet publish -c release

# Final stage / image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY --from=build /src/dapr.sensors.actors/bin/release/net5.0/publish ./
ENTRYPOINT ["dotnet", "dapr.sensors.actors.dll"]