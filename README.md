# 🎵 SetlistManager

A modern web application for managing **setlists, songs, and artists** with real-time collaboration support.

Built with a clean layered architecture using .NET 10, Blazor WebAssembly, and PostgreSQL.

---

## 📦 Solution Structure

| Project | Description |
|----------|-------------|
| **SetlistManager.Api** | ASP.NET Core Web API — backend with REST endpoints and SignalR hub |
| **SetlistManager.App** | Blazor WebAssembly — frontend SPA (PWA-ready) |
| **SetlistManager.Business** | Business logic layer — services, domain operations |
| **SetlistManager.Data** | Data access layer — EF Core, PostgreSQL, migrations |
| **SetlistManager.Common** | Shared models and utilities |
| **SetlistManager.Resources** | Localization resource files |

---

## 🏗 Architecture Overview

- Clean layered architecture
- REST API + SignalR Hub
- JWT-based authentication
- Entity Framework Core with PostgreSQL
- Blazor WebAssembly frontend
- Docker-ready

---

## 🛠 Technologies

- **.NET 10** (`net10.0`)
- **Blazor WebAssembly**
- **MudBlazor**
- **ASP.NET Core Web API**
- **Swagger / OpenAPI**
- **Entity Framework Core**
- **PostgreSQL (Npgsql)**
- **ASP.NET Core Identity**
- **JWT Bearer Authentication**
- **SignalR**
- **Docker**

---

## ✅ Prerequisites

Make sure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- (Optional) [Docker](https://www.docker.com/)

---

# ⚙ Configuration

## 🔐 API Configuration (`SetlistManager.Api`)

For local development, configure secrets using **User Secrets**:

```bash
cd SetlistManager.Api

dotnet user-secrets set "ConnectionStrings:SetlistManagerDB" "Host=localhost;Database=setlistmanager;Username=<user>;Password=<password>"
dotnet user-secrets set "Jwt:SecretKey" "<your-jwt-secret>"
dotnet user-secrets set "Jwt:Issuer" "<issuer>"
dotnet user-secrets set "Jwt:Audience" "<audience>"
```

### Additional Configuration Sections

These can be defined in **User Secrets** or `appsettings.json`:

| Section | Purpose |
|----------|----------|
| `ConnectionStrings:SetlistManagerDB` | PostgreSQL connection string |
| `Jwt` | JWT signing & validation settings |
| `Genius` | Genius API integration |
| `App` | General application options |
| `Brevo` | Email service configuration |

---

## 🌐 Client Configuration (`SetlistManager.App`)

Edit:

```
SetlistManager.App/wwwroot/appsettings.json
```

Example:

```json
{
  "SetlistManager.Api": {
    "BaseUrl": "https://localhost:7143/api",
    "BaseHubUrl": "https://localhost:7143/hubs"
  }
}
```

---

# 🚀 Running Locally

## 1️⃣ Apply Database Migrations

```bash
cd SetlistManager.Api
dotnet ef database update --project ../SetlistManager.Data
```

## 2️⃣ Start the API

```bash
cd SetlistManager.Api
dotnet run
```

Swagger UI:
```
https://localhost:7143/swagger
```

## 3️⃣ Start the Blazor App

```bash
cd SetlistManager.App
dotnet run
```

---

# 🐳 Running with Docker

Each project includes its own `Dockerfile`.

## 🔹 Build & Run API

```bash
docker build -f SetlistManager.Api/Dockerfile -t setlistmanager-api .
docker run -p 8080:8080 setlistmanager-api
```

## 🔹 Build & Run App (served via nginx)

```bash
docker build -f SetlistManager.App/Dockerfile -t setlistmanager-app .
docker run -p 8080:8080 setlistmanager-app
```

---

# 📄 License

This project is intended for **educational and personal use**.

---

**Author:** Adam Lahuta  
**Built with ❤️ using .NET**