# Use the official .NET image from the Docker Hub
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WebRTCService.csproj", "./"]
RUN dotnet restore "WebRTCService.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "WebRTCService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebRTCService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebRTCService.dll"]
