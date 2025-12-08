# Stage 1: Build - Uses .NET SDK to build and publish the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# --- Step 1: Copy .csproj files for dependency restoration ---
# NOTE: The paths are relative to the Solution Root (where you run the build command from).
COPY SetlistManager.App/SetlistManager.App.csproj SetlistManager.App/
COPY SetlistManager.Common/SetlistManager.Common.csproj SetlistManager.Common/
# Restore the main application's dependencies
RUN dotnet restore "SetlistManager.App/SetlistManager.App.csproj"

# --- Step 2: Copy all source code and build ---
# Copy the entire solution source (assuming you run 'docker build' from the solution root)
COPY . .
# Change directory to the app project
WORKDIR "/src/SetlistManager.App"

# Build the application
RUN dotnet build "SetlistManager.App.csproj" -c Release -o /app/build

# Stage 2: Publish - Publishes the built application
FROM build AS publish
# Change directory to the app project
WORKDIR "/src/SetlistManager.App"
# Publish the application (output to /app/publish)
RUN dotnet publish "SetlistManager.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final - Uses Nginx to serve the static Blazor WebAssembly files
FROM nginx:alpine AS final
# Set the working directory for Nginx to serve files from
WORKDIR /usr/share/nginx/html

# Copy the published Blazor WASM files
COPY --from=publish /app/publish/wwwroot .

# Copy the nginx configuration file (relative to where the docker build command is executed, i.e., Solution Root)
COPY SetlistManager.App/nginx.conf /etc/nginx/nginx.conf

# Expose the port Nginx will listen on
EXPOSE 8080

# Command to start Nginx in the foreground
CMD ["nginx", "-g", "daemon off;"]