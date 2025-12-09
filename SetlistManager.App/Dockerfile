# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all project files for dependency resolution
COPY SetlistManager.App/*.csproj ./SetlistManager.App/
COPY SetlistManager.Common/*.csproj ./SetlistManager.Common/
COPY SetlistManager.Business/*.csproj ./SetlistManager.Business/
COPY SetlistManager.Resources/*.csproj ./SetlistManager.Resources/

# Restore dependencies
RUN dotnet restore "./SetlistManager.App/SetlistManager.App.csproj"

# Copy entire source
COPY SetlistManager.App/. ./SetlistManager.App/
COPY SetlistManager.Common/. ./SetlistManager.Common/
COPY SetlistManager.Business/. ./SetlistManager.Business/
COPY SetlistManager.Resources/. ./SetlistManager.Resources/

# Build
WORKDIR "/src/SetlistManager.App"
RUN dotnet build "SetlistManager.App.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SetlistManager.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage - use nginx to serve static files
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copy published Blazor app
COPY --from=publish /app/publish/wwwroot .

# Copy nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Expose port 8080 (Google Cloud Run default)
EXPOSE 8080

# Start nginx
CMD ["nginx", "-g", "daemon off;"]