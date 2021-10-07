FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ./dapr.sensors.interfaces/dapr.sensors.interfaces.csproj ./dapr.sensors.interfaces/dapr.sensors.interfaces.csproj
RUN dotnet restore ./dapr.sensors.interfaces/dapr.sensors.interfaces.csproj

# Copy csproj and restore as distinct layers
COPY ./dapr.sensors.average/dapr.sensors.average.csproj ./dapr.sensors.average/dapr.sensors.average.csproj
RUN dotnet restore ./dapr.sensors.average/dapr.sensors.average.csproj

# Copy everything else and build website
COPY ../dapr.sensors.interfaces/ ./dapr.sensors.interfaces/
COPY ./dapr.sensors.average/ ./dapr.sensors.average/
WORKDIR /src/dapr.sensors.average/
RUN dotnet publish -c release

# Final stage / image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY --from=build /src/dapr.sensors.average/bin/release/net5.0/publish ./
ENTRYPOINT ["dotnet", "dapr.sensors.average.dll"]