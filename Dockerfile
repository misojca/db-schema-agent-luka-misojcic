# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first so the restore layer is cached independently of
# source changes.  Order: leaf → root of the dependency graph.
COPY demo/src/AdventureWorks.Domain/AdventureWorks.Domain.csproj \
     demo/src/AdventureWorks.Domain/
COPY demo/src/AdventureWorks.Application/AdventureWorks.Application.csproj \
     demo/src/AdventureWorks.Application/
COPY demo/src/AdventureWorks.Infrastructure/AdventureWorks.Infrastructure.csproj \
     demo/src/AdventureWorks.Infrastructure/
COPY demo/src/AdventureWorks.Api/AdventureWorks.Api.csproj \
     demo/src/AdventureWorks.Api/

# Restore only when .csproj files change (cached layer).
RUN dotnet restore demo/src/AdventureWorks.Api/AdventureWorks.Api.csproj

# Copy the full source tree and publish a self-contained release artifact.
COPY demo/src/ demo/src/
RUN dotnet publish demo/src/AdventureWorks.Api/AdventureWorks.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy only the published output — SDK is not shipped in the final image.
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AdventureWorks.Api.dll"]
