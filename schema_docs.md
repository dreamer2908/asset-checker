# Database Schema & Entity Relationship Documentation (`schema_docs.md`)

This document describes the SQL Server database schema, Entity Framework Core models (`AssetChecker.Models`), relationship mappings, SQL query logic, and field lineages used by **Asset-Checker Web** (`Asset-Checker.Backend`).

---

## 1. Overview of Database Tables & Entity Classes

The application interacts with four core database tables in Microsoft SQL Server:

| Table Name | Entity Class File | Namespace | Description |
|---|---|---|---|
| `ASTMB` | `Astmb.cs` | `AssetChecker.Models` | **Asset Master Table** — Contains master asset specifications, vendor codes, and management category flags. |
| `ASTMC` | `Astmc.cs` | `AssetChecker.Models` | **Asset Custody Table** — Stores asset custody records, custodian employee code, department code, location, quantity, and notes. |
| `CMSME` | `Cmsme.cs` | `AssetChecker.Models` | **Department Master Table** — Stores department codes and department names. |
| `CMSMV` | `Cmsmv.cs` | `AssetChecker.Models` | **Employee Master Table** — Stores employee codes, full names, and department assignments. |

---

## 2. Entity Relationship Diagram (ERD)

```text
 +------------------------+
 |         ASTMB          |
 | (Asset Master Table)   |
 +------------------------+
 | PK: MB001              |<-------------------+
 |     MB002 (資產名稱)    |                    |
 |     MB003 (資產規格)    |                    |
 |     MB007 (供應廠商)    |                    |
 |     MB008 (供應商簡稱)  |                    |
 |     MB013 (管理區分)    |                    |
 |     MB017 (Scrap Status)|                    |
 |     MB039 (Active Flag) |                    |
 +------------------------+                    |
                                               | (INNER JOIN: a.MB001 = b.MC001)
 +------------------------+                    |
 |         ASTMC          |                    |
 | (Asset Custody Table)  |                    |
 +------------------------+                    |
 | PK: MC001, MC002, MC003|--------------------+
 | FK: MC001 ------------>| ASTMB.MB001 (Asset ID)
 | FK: MC002 ------------>| CMSME.ME001 (Department Code)
 | FK: MC003 ------------>| CMSMV.MV001 (Employee / Custodian Code)
 |     MC004 (Quantity > 0)
 |     MC005 (備註)        |
 |     MC006 (放置地點)    |
 +------------------------+
     |                |
     | (LEFT JOIN     | (LEFT JOIN
     |  b.MC002=      |  b.MC003=
     |  c.ME001)      |  d.MV001)
     v                v
+---------------+  +---------------+
|     CMSME     |  |     CMSMV     |
| (Department)  |  |  (Employee)   |
+---------------+  +---------------+
| PK: ME001     |  | PK: MV001     |
|     ME002     |  |     MV002     |
|  (保管人部門)  |  |    (姓名)     |
|               |  |     MV004 (Dept)
+---------------+  +---------------+
```

---

## 3. SQL Query & Filtering Logic

### 3.1 Main Asset Search Query (`GetFilteredAssetQuery`)

In `AssetsController.cs`, the main asset list query compiles to the following LINQ/SQL structure:

```sql
SELECT a.MB001 AS 資產編號,
       a.MB002 AS 資產名稱,
       a.MB003 AS 資產規格,
       b.MC003 AS 保管人,
       d.MV002 AS 姓名,
       b.MC002 AS 保管代號,
       c.ME002 AS 保管人部門,
       b.MC006 AS 放置地點,
       a.MB007 AS 供應廠商,
       a.MB008 AS 供應商簡稱,
       a.MB013 AS 管理區分,
       b.MC005 AS 備註
FROM ASTMB a
INNER JOIN ASTMC b ON a.MB001 = b.MC001
LEFT JOIN CMSME c ON b.MC002 = c.ME001
LEFT JOIN CMSMV d ON b.MC003 = d.MV001
WHERE b.MC004 > 0
  AND a.MB013 IS NOT NULL AND a.MB013 <> N''
  AND a.MB017 = N''
  AND a.MB013 NOT IN (N'P', N'A')
  AND a.MB039 = 'Y'
```

#### Filtering Rules:
1. `b.MC004 > 0`: Filters for active custody allocations with quantity greater than zero.
2. `a.MB013 <> ''`: Management category code must be non-empty.
3. `a.MB017 = ''`: Asset must not be scrapped or disposed.
4. `a.MB013 NOT IN ('P', 'A')`: Excludes legacy internal system categories `P` and `A`.
5. `a.MB039 = 'Y'`: Asset record must be marked active.

---

### 3.2 Custodian Search Query (`GetCustodians`)

Used for populating custodian selection dropdowns in search filters and the edit modal:

```sql
SELECT d.MV001 AS 保管人,
       d.MV002 AS 姓名,
       d.MV004 AS 保管代號,
       c.ME002 AS 保管人部門
FROM CMSMV d
LEFT JOIN CMSME c ON d.MV004 = c.ME001
WHERE (LOWER(TRIM(d.MV001)) LIKE @pattern OR LOWER(TRIM(d.MV002)) LIKE @pattern)
ORDER BY d.MV001
```

*Note:* Custodian entries saved in local `bookmarks.ini` are retrieved via `IniSettingsService` and pinned to the top of page 1 results (`IsBookmarked = true`).

---

### 3.3 Department Search Query (`GetDepartments`)

Used for populating department selection dropdowns:

```sql
SELECT c.ME001 AS 保管代號,
       c.ME002 AS 保管人部門
FROM CMSME c
WHERE (LOWER(TRIM(c.ME001)) LIKE @pattern OR LOWER(TRIM(c.ME002)) LIKE @pattern)
ORDER BY c.ME001
```

*Note:* Department entries saved in local `bookmarks.ini` are retrieved via `IniSettingsService` and pinned to the top of page 1 results (`IsBookmarked = true`).

---

## 4. Data Updates & Write Operations

### Asset Custody Edit (`PUT /api/assets/{id}`)

When editing asset custody details, the backend performs validations before executing an update on `ASTMC`:

```sql
UPDATE ASTMC 
SET MC002 = @deptCode, 
    MC003 = @custodianCode, 
    MC005 = @remark, 
    MC006 = @location 
WHERE TRIM(MC001) = @assetId
```

#### Validation & Referential Integrity Enforcements:
1. **Custodian Validation:** Asserts that `MC003` (`保管人`) exists in `CMSMV.MV001`.
2. **Department Validation:** Asserts that `MC002` (`保管代號`) exists in `CMSME.ME001`.
3. **Atomic Target Update:** Updates only custody assignment columns (`MC002`, `MC003`, `MC005`, `MC006`) in `ASTMC` without modifying master asset specifications in `ASTMB`.

---

## 5. Main Table Field Mapping

The table below traces each UI column on the frontend main asset grid to its database field:

| UI Column Header | DTO Property | Database Table | Source Column | Column Data Type & Notes |
|---|---|---|---|---|
| **資產編號** | `資產編號` | `ASTMB` | `a.MB001` | `CHAR(40)` — Asset Barcode / ID Primary Key |
| **資產名稱** | `資產名稱` | `ASTMB` | `a.MB002` | `VARCHAR(120)` — Asset item description |
| **資產規格** | `資產規格` | `ASTMB` | `a.MB003` | `VARCHAR(120)` — Asset technical specification |
| **保管人** | `保管人` | `ASTMC` | `b.MC003` | `CHAR(10)` — Custodian employee code (e.g. V011875) |
| **姓名** | `姓名` | `CMSMV` | `d.MV002` | `VARCHAR(30)` — Custodian employee full name |
| **保管代號** | `保管代號` | `ASTMC` | `b.MC002` | `CHAR(10)` — Custodian department code (e.g. VEQ0300) |
| **保管人部門** | `保管人部門` | `CMSME` | `c.ME002` | `VARCHAR(40)` — Custodian department name |
| **放置地點** | `放置地點` | `ASTMC` | `b.MC006` | `VARCHAR(40)` — Physical asset storage location |
| **供應廠商** | `供應廠商` | `ASTMB` | `a.MB007` | `VARCHAR(10)` — Supplier / vendor code |
| **供應商簡稱** | `供應商簡稱` | `ASTMB` | `a.MB008` | `VARCHAR(30)` — Vendor abbreviation |
| **管理區分** | `管理區分` | `ASTMB` | `a.MB013` | `VARCHAR(1)` — Management category (`G`, `M`, `L`, `I`, `K`) |
| **備註** | `備註` | `ASTMC` | `b.MC005` | `VARCHAR(255)` — Custody remarks / notes |

---

## 6. Configuration Security Notice

All database connection strings and configuration settings must use secure placeholders (e.g. `Server=YOUR_SERVER;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;`). Never include secrets or production passwords in documentation files or source control.
