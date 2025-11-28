# ========== 1. Build stage ==========
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["WordleHelper.Web/WordleHelper.Web.csproj", "WordleHelper.Web/"]
RUN dotnet restore "./WordleHelper.Web/WordleHelper.Web.csproj"

COPY . .
WORKDIR "/src/WordleHelper.Web"
RUN dotnet build "./WordleHelper.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ========== 2. Publish stage ==========
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c Release -o /app/publish

# ========== 3. Runtime stage (Nginx) ==========
FROM nginx:1.27-alpine

# Copy custom nginx configuration
COPY nginx/default.conf /etc/nginx/conf.d/default.conf

# Copy Blazor published static files
COPY --from=publish /app/publish/wwwroot /usr/share/nginx/html

# Expose HTTP
EXPOSE 80

# Add health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --quiet --tries=1 --spider http://localhost/index.html || exit 1

CMD ["nginx", "-g", "daemon off;"]