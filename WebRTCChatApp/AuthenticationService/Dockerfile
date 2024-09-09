# # Stage 1: Build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ../SharedData/SharedData.csproj ./
COPY AuthenticationService/AuthenticationService.csproj ./AuthenticationService/

# Restore dependencies for both projects
RUN dotnet restore ./AuthenticationService/AuthenticationService.csproj

# Copy the rest of the source code
COPY ../SharedData ./SharedData
COPY AuthenticationService ./AuthenticationService

# Build and publish the AuthenticationService project
WORKDIR /src/AuthenticationService
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

# Set the entry point for the container
ENTRYPOINT ["dotnet", "AuthenticationService.dll"]