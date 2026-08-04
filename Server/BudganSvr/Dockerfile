FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# procps provides `ps`, required by Rider to enumerate processes when attaching the debugger
USER root
RUN apt-get update && apt-get install -y --no-install-recommends procps \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BudganSvr/BudganSvr.csproj", "BudganSvr/"]
RUN dotnet restore "BudganSvr/BudganSvr.csproj"
COPY . .
WORKDIR "/src/BudganSvr"
RUN dotnet build "./BudganSvr.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS debug
WORKDIR /src
COPY ["BudganSvr/BudganSvr.csproj", "BudganSvr/"]
RUN dotnet restore "BudganSvr/BudganSvr.csproj"
COPY . .
WORKDIR "/src/BudganSvr"
RUN dotnet build "./BudganSvr.csproj" -c Debug -o /app/build

# procps provides `ps`, required by Rider to enumerate processes for attachment
# vsdbg is Microsoft's .NET debugger; Rider also uses it when configured with vsdbg transport
RUN apt-get update && apt-get install -y --no-install-recommends curl unzip procps \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg \
    && rm -rf /var/lib/apt/lists/*

# Rider uploads its own DebuggerWorker here on first attach; the volume in compose.yaml
# persists it so subsequent debug sessions skip the upload.
RUN mkdir -p /jetbrains-debugger

EXPOSE 8080
WORKDIR /app/build
ENTRYPOINT ["dotnet", "BudganSvr.dll"]

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BudganSvr.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BudganSvr.dll"]
