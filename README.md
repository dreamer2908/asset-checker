# LegacyWebBridge - Unified ASP.NET Core Asset Checker Web Application

## Overview
`LegacyWebBridge` is a unified **ASP.NET Core (.NET 8)** web application that consolidates a legacy static frontend (previously hosted on IIS) and a standalone Node.js Express API backend into a **single process operating on a single configurable HTTP port**.

---

## Project Structure

```text
LegacyWebBridge/
├── Controllers/
│   └── AssetsController.cs          # Unified ASP.NET Core Controller (/assets, /export)
├── Models/
│   ├── AppDbContext.cs               # EF Core DbContext for SQL Server
│   ├── Astmb.cs                      # Asset Master Table Entity (ASTMB)
│   ├── Astmc.cs                      # Asset Custody Table Entity (ASTMC)
│   ├── Cmsme.cs                      # Department Table Entity (CMSME)
│   └── Cmsmv.cs                      # Employee Table Entity (CMSMV)
├── wwwroot/                          # Static Frontend Files (React SPA bundle)
│   ├── index.html                    # Single Page Application entry point
│   ├── favicon.ico
│   ├── manifest.json
│   └── static/
│       ├── css/
│       └── js/                       # Compiled React JS bundle
├── appsettings.json                  # Application configuration (Port, ConnectionStrings)
├── appsettings.Development.json      # Development environment settings
├── LegacyWebBridge.csproj            # .NET 8 Project file with NuGet dependencies
├── Program.cs                        # Web host startup & middleware pipeline
├── README.md                         # Project documentation
└── schema_docs.md                    # Database schema & entity relationship mapping
```

---

## Environment & Prerequisites

- **.NET Runtime / SDK:** .NET 8.0 SDK or ASP.NET Core 8.0 Runtime
- **Database:** Microsoft SQL Server 2016 or newer (SQL Server 2016+)
- **Operating Systems Supported:**
  - **Windows Server:** Windows Server 2016 / 2019 / 2022
  - **Linux:** Ubuntu 24.04 LTS / Ubuntu 22.04 LTS / Debian 12

---

## Configuration

Configuration parameters are managed in `appsettings.json` (or via environment variables).

```json
{
  "Port": 12345,
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER_IP;Database=LegacyTestDB;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Port Configuration Options
The application binds to a single port configured in one of three ways (evaluated in order):
1. **Command Line Argument:** `--Port 12345` or `--urls "http://0.0.0.0:12345"`
2. **Environment Variable:** `ASPNETCORE_URLS="http://0.0.0.0:12345"` or `Port=12345`
3. **AppSettings:** `"Port": 12345` in `appsettings.json`

---

## Running the Application

### 1. Running on Windows (Windows Server 2016+)

#### Command Line / PowerShell:
```powershell
# Build project
dotnet build -c Release

# Run application
dotnet run --project LegacyWebBridge.csproj
```

#### Running as IIS In-Process / Windows Service:
Publish the project to a target directory:
```powershell
dotnet publish -c Release -o C:\inetpub\wwwroot\LegacyWebBridge
```
Point IIS App Pool or Windows Service executable to `LegacyWebBridge.exe`.

---

### 2. Running on Linux (Ubuntu 24.04 LTS)

#### Terminal / Bash:
```bash
# Build project
dotnet build -c Release

# Run application on default port (e.g. 12345)
dotnet run --project LegacyWebBridge.csproj
```

#### Running via systemd service:
Create `/etc/systemd/system/legacywebbridge.service`:
```ini
[Unit]
Description=LegacyWebBridge ASP.NET Core Application
After=network.target

[Service]
WorkingDirectory=/var/www/LegacyWebBridge
ExecStart=/usr/bin/dotnet /var/www/LegacyWebBridge/LegacyWebBridge.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=legacywebbridge
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=Port=12345

[Install]
WantedBy=multi-user.target
```

Enable and start the service:
```bash
sudo systemctl enable legacywebbridge
sudo systemctl start legacywebbridge
```

---

## Search Criteria & API Reference

The unified backend exposes two primary endpoints:

### 1. `GET /assets` or `GET /api/assets`
Returns a paginated JSON response of assets matching the query parameters.

**Query Parameters:**
| Parameter | Type | Default | Description |
|---|---|---|---|
| `page` | `int` | `1` | Page number (1-indexed). |
| `pageSize` | `int` | `20` | Number of items per page. |
| `managerType` | `string` | `null` | Management category code (`G`, `M`, `L`, `I`, `K`). Case-insensitive. |
| `assetId` | `string` | `null` | Asset ID search term. Supports wildcards and is case-insensitive. |

**Wildcard Search Rules for `assetId`:**
- `?` or `_`: Matches any **single character**.
- `*` or `%`: Matches **zero or more characters**.
- **Standard Text Search**: Searching `V25` without wildcards automatically performs a substring search (`%V25%`).

**Case Insensitivity:**
All text searches (`assetId` and `managerType`) are **case-insensitive** (e.g., `v25*` matches `V25-2E162-0002`, `g` matches `G`).

**Example Response:**
```json
{
  "data": [
    {
      "資產編號": "V25-2E162-0002",
      "資產名稱": "蒸汽烫斗/BÀN ỦI HƠI NƯỚC",
      "資產規格": "GC518/29 Philips",
      "保管人": "V011875",
      "姓名": "TRINH LE NGUYEN",
      "保管代號": "VEQ0300",
      "保管人部門": "QA Sec. 品保課",
      "放置地點": "CS4 QA PK HàNG",
      "供應廠商": "A-BA002-VN",
      "供應商簡稱": "BA",
      "管理區分": "G",
      "備註": ""
    }
  ],
  "total": 345
}
```

---

### 2. `GET /export` or `GET /api/export`
Exports all matching asset records into an Excel file (`assets.xlsx`).

**Query Parameters:**
- `managerType` (optional, case-insensitive)
- `assetId` (optional, wildcard enabled, case-insensitive)

**Response:** Binary `.xlsx` spreadsheet (`application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`).
