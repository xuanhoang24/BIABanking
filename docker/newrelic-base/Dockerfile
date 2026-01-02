FROM mcr.microsoft.com/dotnet/aspnet:9.0

RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
    && wget -O - https://download.newrelic.com/install/newrelic-cli/scripts/install.sh | bash \
    && apt-get update \
    && apt-get install -y newrelic-dotnet-agent \
    && rm -rf /var/lib/apt/lists/*

ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER="{36032161-FFC0-4B61-B559-F6C5D41BAE5A}" \
    CORECLR_NEWRELIC_HOME="/usr/local/newrelic-dotnet-agent" \
    CORECLR_PROFILER_PATH="/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so"

WORKDIR /app
EXPOSE 8080
