# Stage 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY OmniERP.sln .
COPY src/OmniERP.Api/OmniERP.Api.csproj src/OmniERP.Api/
COPY src/OmniERP.Application/OmniERP.Application.csproj src/OmniERP.Application/
COPY src/OmniERP.Domain/OmniERP.Domain.csproj src/OmniERP.Domain/
COPY src/OmniERP.Infrastructure/OmniERP.Infrastructure.csproj src/OmniERP.Infrastructure/

RUN dotnet restore OmniERP.sln

COPY src/ src/
RUN dotnet publish src/OmniERP.Api/OmniERP.Api.csproj -c Release -o /app/publish --no-restore

# Stage 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 5000
ENTRYPOINT ["dotnet", "OmniERP.Api.dll"]
