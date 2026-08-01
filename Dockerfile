# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for layer caching
COPY DomainLayer/DomainLayer.csproj DomainLayer/
COPY ApplicationLayer/ApplicationLayer.csproj ApplicationLayer/
COPY InfrastructureLayer/InfrastructureLayer.csproj InfrastructureLayer/
COPY mentor-booking-be/ControllerLayer.csproj mentor-booking-be/

RUN dotnet restore mentor-booking-be/ControllerLayer.csproj

# Copy all source code
COPY . .

# Publish the app
RUN dotnet publish mentor-booking-be/ControllerLayer.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

# Listen on port 8080 (Azure Container Apps default)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ControllerLayer.dll"]
