# ==========================
# Build stage
# ==========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything
COPY . .

# Restore dependencies and publish
RUN dotnet restore
RUN dotnet publish -c Release -o out

# ==========================
# Runtime stage
# ==========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published output from build stage
COPY --from=build /app/out .

# Expose port (your app port)
EXPOSE 10000

# Start the application
ENTRYPOINT ["dotnet", "DHLManagementSystem.dll"]