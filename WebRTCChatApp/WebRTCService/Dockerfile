# # Stage 1: Build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /src/WebRTCService

# Copy the project file and restore dependencies
COPY WebRTCService.csproj ./
RUN dotnet restore

# Copy the rest of the files
COPY . .

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# Build the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory in the container
WORKDIR /app/publish

# Copy the published application
COPY --from=build /app/publish .

# Expose the web port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "WebRTCService.dll"]
