FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.csproj .
RUN dotnet restore

COPY . .
WORKDIR /src

# publish optimized but do NOT trim (trimming can remove assemblies used via reflection)
RUN dotnet publish -c Release -o /app/publish \
    /p:EnvironmentName=Production

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:5145

COPY --from=build /app/publish .

# Create directory for SQLite and set permissions
RUN mkdir -p /app/data && \
    chown -R 1000:1000 /app/data && \
    chmod -R 755 /app/data

# Install wget for healthcheck
RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/*

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD wget -q --spider http://localhost:5145/health || exit 1

EXPOSE 5145
ENTRYPOINT ["dotnet", "FootballAPI.dll"]