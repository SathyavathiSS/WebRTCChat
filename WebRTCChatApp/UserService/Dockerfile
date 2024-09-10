# # Stage 1: Build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ../SharedData/SharedData.csproj ./SharedData/
COPY UserService/UserService.csproj ./UserService/

# Restore dependencies for both projects
RUN dotnet restore ./UserService/UserService.csproj

# Copy the rest of the source code
COPY ../SharedData ./SharedData
COPY UserService ./UserService

# Build and publish the project
WORKDIR /src/UserService
RUN dotnet publish -c Release -o /app/out

# Stage 2: Publish the project
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Install tzdata for time zone management
RUN apt-get update && \
    apt-get install -y tzdata && \
    # Set the time zone to Asia/Kolkata (UTC+5:30)
    ln -sf /usr/share/zoneinfo/Asia/Kolkata /etc/localtime && \
    dpkg-reconfigure --frontend noninteractive tzdata

# Copy the build output from the previous stage
COPY --from=build /app/out .

# Expose the web port
EXPOSE 8080

# Set the entry point for the container
ENTRYPOINT ["dotnet", "UserService.dll"]    
