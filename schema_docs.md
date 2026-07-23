# Database Schema & Entity Relationship Documentation (`schema_docs.md`)

This document describes the database tables, Entity Framework Core models (`AppDbContext.cs`), relationship mappings, and field lineages used by the **LegacyWebBridge** asset management system.

---

## 1. Overview of Database Tables

The application queries four core tables in Microsoft SQL Server:

| Table Name | Entity Class | Description |
|---|---|---|
| `ASTMB` | `Astmb.cs` | **Asset Master Table** — Stores primary asset details, specifications, vendor codes, and management category flags. |
| `ASTMC` | `Astmc.cs` | **Asset Custody Table** — Stores custody location, custody quantity, custodian employee code, department code, and notes. |
| `CMSME` | `Cmsme.cs` | **Department Master Table** — Stores department codes and department names. |
| `CMSMV` | `Cmsmv.cs` | **Employee Master Table** — Stores employee IDs, full names, and employee master attributes. |

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
 | FK: MC001 ------------>| ASTMB.MB001
 | FK: MC002 ------------>| CMSME.ME001 (Department Key)
 | FK: MC003 ------------>| CMSMV.MV001 (Employee Key)
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
+---------------+  +---------------+
```

---

## 3. SQL Join & Filtering Logic

In `AssetsController.cs` (method `GetFilteredAssetQuery`), the LINQ query compiles to the following SQL join structure:

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

### Filtering Business Rules
1. `b.MC004 > 0`: Only assets with an active custody quantity greater than zero are included.
2. `a.MB013 <> ''`: Management category code must be non-empty.
3. `a.MB017 = ''`: Asset must not be marked as scrapped/disposed.
4. `a.MB013 NOT IN ('P', 'A')`: Excludes legacy internal system category codes `P` and `A`.
5. `a.MB039 = 'Y'`: Asset must be active.

---

## 4. Frontend Main Table Field Mapping

The main data table rendered on the React web page displays 12 columns. The table below traces each displayed field to its underlying database table and column:

| Column # | Web Page Column Header | Output Property | Source Table | Source Column | Column Type & Description |
|---|---|---|---|---|---|
| 1 | **資產編號** | `資產編號` | `ASTMB` | `a.MB001` | `CHAR(40)` — Asset ID / Barcode primary key. |
| 2 | **資產名稱** | `資產名稱` | `ASTMB` | `a.MB002` | `VARCHAR(120)` — Asset item name (e.g. 蒸汽烫斗/BÀN ỦI HƠI NƯỚC). |
| 3 | **資產規格** | `資產規格` | `ASTMB` | `a.MB003` | `VARCHAR(120)` — Asset technical specification / model number. |
| 4 | **保管人** | `保管人` | `ASTMC` | `b.MC003` | `CHAR(10)` — Custodian employee code (e.g. V011875). |
| 5 | **姓名** | `姓名` | `CMSMV` | `d.MV002` | `VARCHAR(30)` — Custodian employee full name (e.g. TRINH LE NGUYEN). |
| 6 | **保管代號** | `保管代號` | `ASTMC` | `b.MC002` | `CHAR(10)` — Custodian department code (e.g. VEQ0300). |
| 7 | **保管人部門** | `保管人部門` | `CMSME` | `c.ME002` | `VARCHAR(40)` — Custodian department name (e.g. QA Sec. 品保課). |
| 8 | **放置地點** | `放置地點` | `ASTMC` | `b.MC006` | `VARCHAR(40)` — Physical asset storage location (e.g. CS4 QA PK HàNG). |
| 9 | **供應廠商** | `供應廠商` | `ASTMB` | `a.MB007` | `VARCHAR(10)` — Supplier / vendor code (e.g. A-BA002-VN). |
| 10 | **供應商簡稱** | `供應商簡稱` | `ASTMB` | `a.MB008` | `VARCHAR(30)` — Supplier / vendor abbreviated name (e.g. BA). |
| 11 | **管理區分** | `管理區分` | `ASTMB` | `a.MB013` | `VARCHAR(1)` — Management category (`G`: General Affairs, `M`: Production, `L`: Lab, `I`: IT, `K`: Sample Dev). |
| 12 | **備註** | `備註` | `ASTMC` | `b.MC005` | `VARCHAR(255)` — Custody remarks / notes. |
