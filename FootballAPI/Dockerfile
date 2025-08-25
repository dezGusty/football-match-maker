# FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# WORKDIR /app

# COPY . ./

# RUN dotnet restore
# RUN dotnet publish -c Release -o out

# FROM mcr.microsoft.com/dotnet/aspnet:9.0
# WORKDIR /app
# COPY --from=build /app/out .

# EXPOSE 5145

# ENTRYPOINT ["dotnet", "FootballAPI.dll"]

# ...existing code...
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

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:5145/health || exit 1

EXPOSE 5145
ENTRYPOINT ["dotnet", "FootballAPI.dll"]
# ...existing code...