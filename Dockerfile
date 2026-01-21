## Build sorgente
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine3.20 AS publish
WORKDIR /src
COPY . ./
RUN dotnet publish -c Release -o /app/publish --runtime linux-musl-x64 --no-self-contained

## SDK pyroscope
FROM --platform=linux/amd64 pyroscope/pyroscope-dotnet:0.10.0-musl AS sdk

## Ambiente d'esecuzione
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:9.0-alpine3.20 AS base
RUN apk add --no-cache curl

# Configurazione per la globalizzazione
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add --no-cache icu-data-full icu-libs tzdata

# Copia della build
WORKDIR /app
COPY --from=publish /app/publish .

# Aggiunta librerie e variabili per il profiling
COPY --from=sdk /Pyroscope.Profiler.Native.so ./Pyroscope.Profiler.Native.so
COPY --from=sdk /Pyroscope.Linux.ApiWrapper.x64.so ./Pyroscope.Linux.ApiWrapper.x64.so

ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={BD1A650D-AC5D-4896-B64F-D6FA25D6B26A}
ENV CORECLR_PROFILER_PATH=/app/Pyroscope.Profiler.Native.so
ENV LD_PRELOAD=/app/Pyroscope.Linux.ApiWrapper.x64.so

# ENV PYROSCOPE_SERVER_ADDRESS=https://graph.fm-technology.it/pyroscope
# ENV PYROSCOPE_APPLICATION_NAME=<tenant>.<ambiente>.<applicativo>.fmt
ENV PYROSCOPE_LOG_LEVEL=debug
ENV PYROSCOPE_PROFILING_ENABLED=0
ENV PYROSCOPE_PROFILING_ALLOCATION_ENABLED=true
ENV PYROSCOPE_PROFILING_CONTENTION_ENABLED=true
ENV PYROSCOPE_PROFILING_EXCEPTION_ENABLED=true
ENV PYROSCOPE_PROFILING_HEAP_ENABLED=true

# Esecuzione app
USER app
ENTRYPOINT ["dotnet", "Trasformazioni.dll"]
