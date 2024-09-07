# # Stage 1: Build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY AuthenticationService.csproj ./
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the project
RUN dotnet publish -c Release -o out

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
COPY --from=build /src/out .

# Set the entry point for the container
ENTRYPOINT ["dotnet", "AuthenticationService.dll"]