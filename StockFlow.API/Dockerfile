FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG VERSION=0.0.0-dev
WORKDIR /src

COPY Directory.Build.props ./
COPY StockFlow.API/StockFlow.API.csproj StockFlow.API/
COPY StockFlow.Application/StockFlow.Application.csproj StockFlow.Application/
COPY StockFlow.Domain/StockFlow.Domain.csproj StockFlow.Domain/
COPY StockFlow.Infrastructure/StockFlow.Infrastructure.csproj StockFlow.Infrastructure/
COPY StockFlow.Shared/StockFlow.Shared.csproj StockFlow.Shared/
RUN dotnet restore StockFlow.API/StockFlow.API.csproj

COPY StockFlow.API/ StockFlow.API/
COPY StockFlow.Application/ StockFlow.Application/
COPY StockFlow.Domain/ StockFlow.Domain/
COPY StockFlow.Infrastructure/ StockFlow.Infrastructure/
COPY StockFlow.Shared/ StockFlow.Shared/
RUN dotnet publish StockFlow.API/StockFlow.API.csproj -c Release -o /app/publish --no-restore -p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080 \
    DataFolder=/app/data
RUN mkdir -p /app/data && chown $APP_UID /app/data
COPY --from=build /app/publish .
USER $APP_UID
VOLUME /app/data
EXPOSE 8080
ENTRYPOINT ["dotnet", "StockFlow.API.dll"]
