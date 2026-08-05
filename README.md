# Asset-Checker Web - Unified ASP.NET Core Asset Management Application

## Overview
`Asset-Checker Web` is a unified **ASP.NET Core (.NET 8)** web application that consolidates a React SPA frontend and an ASP.NET Core Web API backend into a **single process operating on a single configurable HTTP port**.

The application provides asset inventory querying with wildcard search, Excel report exports, asset custody record editing with validation, custodian and department search dropdowns with infinite scroll and bookmarking, and local INI settings persistence.

---

## Project Structure

```text
Asset-Checker-Web/
├── Asset-Checker.slnx              # Solution file
├── README.md                       # Main project documentation
├── schema_docs.md                    # Database schema & entity relationship mapping
├── backend/                          # Backend ASP.NET Core (.NET 8) Web API (`Asset-Checker.Backend`)
│   ├── Controllers/
│   │   └── AssetsController.cs       # Unified Web API Controller (Assets, Export, Custodians, Departments, Bookmarks, Edit)
│   ├── Models/
│   │   ├── AppDbContext.cs            # EF Core DbContext for SQL Server
│   │   ├── Astmb.cs                   # Asset Master Table Entity (ASTMB)
│   │   ├── Astmc.cs                   # Asset Custody Table Entity (ASTMC)
│   │   ├── Cmsme.cs                   # Department Master Table Entity (CMSME)
│   │   └── Cmsmv.cs                   # Employee Master Table Entity (CMSMV)
│   ├── Services/
│   │   └── IniSettingsService.cs      # Service for persistence of custodian/department bookmarks (bookmarks.ini)
│   ├── wwwroot/                       # Static Frontend Files (Compiled React SPA bundle)
│   │   ├── index.html                 # Single Page Application entry point
│   │   └── static/                    # Compiled React JS and CSS bundle assets
│   ├── appsettings.json               # Application configuration (Port, ConnectionStrings)
│   ├── appsettings.Development.json      # Development configuration
│   ├── bookmarks.ini                  # Local storage for custodian and department bookmarks
│   ├── Program.cs                     # Web host startup & middleware pipeline
│   └── Asset-Checker.Backend.csproj   # .NET 8 Backend project file
└── frontend/                         # Frontend React SPA Source Code
    ├── public/                        # Public template assets
    ├── src/                           # React component source code
    │   ├── App.js                     # Main application UI (Search filters, data table, pagination)
    │   ├── EditAssetModal.js          # Modal dialog for asset custody editing
    │   ├── App.css
    │   └── index.js                   # Application entry point
    ├── package.json                   # NPM dependencies and build scripts
    └── setupProxy.js                  # Proxy configuration for development API forwarding
```

---

## Environment & Prerequisites

- **.NET Runtime / SDK:** .NET 8.0 SDK or ASP.NET Core 8.0 Runtime
- **Node.js & NPM:** Node.js 18+ (required for frontend React development and build)
- **Database:** Microsoft SQL Server 2016 or newer (SQL Server 2016+)
- **Supported Operating Systems:**
  - **Windows Server:** Windows Server 2016 / 2019 / 2022
  - **Linux:** Ubuntu 24.04 LTS / Ubuntu 22.04 LTS / Debian 12

---

## Configuration

Configuration parameters are defined in `backend/appsettings.json` or overridden via environment variables.

```json
{
  "Port": 12345,
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER_IP;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

> [!NOTE]
> Database connection credentials should never be committed to configuration files in production repositories. Use environment variables or secure secret configuration providers.

### Port Configuration Options
The application binds to a single configurable HTTP port evaluated in the following order of precedence:
1. **Command Line Argument:** `--Port 12345` or `--urls "http://0.0.0.0:12345"`
2. **Environment Variable:** `ASPNETCORE_URLS="http://0.0.0.0:12345"` or `Port=12345`
3. **AppSettings:** `"Port": 12345` in `backend/appsettings.json`

---

## Building and Running the Application

### 1. Backend (.NET 8 Web API)

To build the backend project:
```bash
cd backend
dotnet build -c Release
```

To run the backend locally on the default configured port:
```bash
cd backend
dotnet run
```

---

### 2. Frontend (React SPA)

To install dependencies and build the static frontend assets into `backend/wwwroot/`:
```bash
cd frontend
npm install
npm run build
```

---

### 3. Production Deployment Examples

#### Windows Server Deployment (PowerShell):
```powershell
cd backend
dotnet publish -c Release -o C:\inetpub\wwwroot\Asset-Checker
```
Point IIS App Pool or Windows Service executable to `Asset-Checker.Backend.exe`.

#### Linux systemd Deployment (Ubuntu 24.04 LTS):
Create `/etc/systemd/system/asset-checker.service`:
```ini
[Unit]
Description=Asset-Checker Web Application
After=network.target

[Service]
WorkingDirectory=/var/www/Asset-Checker/backend
ExecStart=/usr/bin/dotnet /var/www/Asset-Checker/backend/Asset-Checker.Backend.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=asset-checker
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=Port=12345

[Install]
WantedBy=multi-user.target
```

Enable and start the service:
```bash
sudo systemctl enable asset-checker
sudo systemctl start asset-checker
```

---

## API Reference

All backend API endpoints are defined in `AssetChecker.Controllers.AssetsController`:

### 1. `GET /assets` or `GET /api/assets`
Returns a paginated JSON list of asset records matching the specified search parameters.

**Query Parameters:**
| Parameter | Type | Default | Description |
|---|---|---|---|
| `page` | `int` | `1` | Page number (1-indexed). |
| `pageSize` | `int` | `20` | Number of items per page. |
| `managerType` | `string` | `null` | Management category code (`G`, `M`, `L`, `I`, `K`). Case-insensitive. |
| `assetId` | `string` | `null` | Asset ID search query. Supports wildcards (`*`, `?`, `%`, `_`) and case-insensitive matching. |

**Wildcard Rules for `assetId`:**
- `?` or `_`: Matches any single character.
- `*` or `%`: Matches zero or more characters.
- **Standard Search:** Searching `V25` without wildcards performs a substring search (`%V25%`).

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

### 2. `PUT /assets/{id}` or `PUT /api/assets/{id}`
Updates the custody record for a specific asset (`ASTMC` table).

**Request Parameters:**
- `id` (path): Asset ID (`資產編號`).

**Request Body:**
```json
{
  "保管人": "V011875",
  "保管代號": "VEQ0300",
  "放置地點": "CS4 QA PK HàNG",
  "備註": "Updated storage area"
}
```

**Validation Rules:**
- Checks if `保管人` exists in employee master (`CMSMV`). Returns HTTP 400 if invalid.
- Checks if `保管代號` exists in department master (`CMSME`). Returns HTTP 400 if invalid.

---

### 3. `GET /export` or `GET /api/export`
Exports matching asset records as an Excel file (`assets.xlsx`).

**Query Parameters:** `managerType`, `assetId`

---

### 4. `GET /api/custodians`
Queries custodians from `CMSMV` (Employee Master) joined with `CMSME` (Department Master).

**Query Parameters:**
| Parameter | Type | Default | Description |
|---|---|---|---|
| `q` | `string` | `null` | Search filter matching custodian employee code (`MV001`) or employee name (`MV002`). |
| `deptCode` | `string` | `null` | Filter by department code (`MV004`). |
| `page` | `int` | `1` | Page number for infinite scroll. |
| `pageSize` | `int` | `20` | Page size. |

> [!TIP]
> Bookmarked custodians are automatically pinned to the top of page 1 search results.

---

### 5. `GET /api/custodians/details/{code}`
Retrieves custodian details by employee code (`MV001`).

---

### 6. `GET /api/departments`
Queries department records from `CMSME`. Supports search term `q` and pagination. Bookmarked departments are pinned to the top of page 1.

---

### 7. Bookmarks Endpoints
- `GET /api/bookmarks` — Retrieves list of bookmarked custodian codes.
- `POST /api/bookmarks/toggle` — Toggles bookmark state for a custodian (`{"custodianCode": "V011875"}`).
- `GET /api/bookmarks/departments` — Retrieves list of bookmarked department codes.
- `POST /api/bookmarks/toggle-dept` — Toggles bookmark state for a department (`{"custodianCode": "VEQ0300"}`).

Bookmarks are saved locally in `backend/bookmarks.ini`.
