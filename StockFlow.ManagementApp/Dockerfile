FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG VERSION=0.0.0-dev
WORKDIR /src

COPY Directory.Build.props ./
COPY StockFlow.ManagementApp/StockFlow.ManagementApp.csproj StockFlow.ManagementApp/
COPY StockFlow.Shared/StockFlow.Shared.csproj StockFlow.Shared/
RUN dotnet restore StockFlow.ManagementApp/StockFlow.ManagementApp.csproj

COPY StockFlow.ManagementApp/ StockFlow.ManagementApp/
COPY StockFlow.Shared/ StockFlow.Shared/
RUN dotnet publish StockFlow.ManagementApp/StockFlow.ManagementApp.csproj -c Release -o /app/publish --no-restore -p:Version=$VERSION

ARG BASE_PATH=/stockflow/management/
ARG API_BASE_URL=/stockflow/api/
ARG PUBLIC_SITE_URL=/stockflow/public/
RUN cd /app/publish/wwwroot \
    && sed -i "s|<base href=\"/\" />|<base href=\"$BASE_PATH\" />|" index.html \
    && printf '{\n  "ApiBaseUrl": "%s",\n  "PublicSiteUrl": "%s"\n}\n' "$API_BASE_URL" "$PUBLIC_SITE_URL" > appsettings.json \
    && rm -f index.html.gz index.html.br appsettings.json.gz appsettings.json.br

FROM nginx:1.27-alpine AS final
COPY StockFlow.ManagementApp/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
EXPOSE 80
