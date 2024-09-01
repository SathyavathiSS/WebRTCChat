# # Stage 1: Build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# # Install the SQL Server tools
# RUN apt-get update && \
#     ACCEPT_EULA=Y apt-get install -y mssql-tools

# # Set the default command for the container
# CMD ["/opt/mssql/bin/sqlservr"]


# USER root

# # Install curl and other necessary packages
# RUN apt-get update && \
#     apt-get install -y curl gnupg unixodbc-dev && \
#     curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
#     curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
#     apt-get update && \
#     ACCEPT_EULA=Y apt-get install -y mssql-tools

# USER mssql


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

COPY --from=build /src/out .

ENTRYPOINT ["dotnet", "AuthenticationService.dll"]
