using System.Data;
using System.Text.Json.Serialization;
using ClosedXML.Excel;
using LegacyWebBridge.Models;
using LegacyWebBridge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegacyWebBridge.Controllers
{
    /// <summary>
    /// Data Transfer Object representing an individual asset item for API responses and Excel export.
    /// Property names correspond to the exact Chinese field names expected by the legacy frontend.
    /// </summary>
    public class AssetDto
    {
        /// <summary>Gets or sets the asset ID (資產編號).</summary>
        [JsonPropertyName("資產編號")]
        public string 資產編號 { get; set; } = string.Empty;

        /// <summary>Gets or sets the asset name (資產名稱).</summary>
        [JsonPropertyName("資產名稱")]
        public string 資產名稱 { get; set; } = string.Empty;

        /// <summary>Gets or sets the asset specification (資產規格).</summary>
        [JsonPropertyName("資產規格")]
        public string 資產規格 { get; set; } = string.Empty;

        /// <summary>Gets or sets the custodian employee code (保管人).</summary>
        [JsonPropertyName("保管人")]
        public string 保管人 { get; set; } = string.Empty;

        /// <summary>Gets or sets the custodian employee name (姓名).</summary>
        [JsonPropertyName("姓名")]
        public string 姓名 { get; set; } = string.Empty;

        /// <summary>Gets or sets the custodian department code (保管代號).</summary>
        [JsonPropertyName("保管代號")]
        public string 保管代號 { get; set; } = string.Empty;

        /// <summary>Gets or sets the custodian department name (保管人部門).</summary>
        [JsonPropertyName("保管人部門")]
        public string 保管人部門 { get; set; } = string.Empty;

        /// <summary>Gets or sets the storage location (放置地點).</summary>
        [JsonPropertyName("放置地點")]
        public string 放置地點 { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier vendor code (供應廠商).</summary>
        [JsonPropertyName("供應廠商")]
        public string 供應廠商 { get; set; } = string.Empty;

        /// <summary>Gets or sets the supplier vendor abbreviation (供應商簡稱).</summary>
        [JsonPropertyName("供應商簡稱")]
        public string 供應商簡稱 { get; set; } = string.Empty;

        /// <summary>Gets or sets the management category code (管理區分).</summary>
        [JsonPropertyName("管理區分")]
        public string 管理區分 { get; set; } = string.Empty;

        /// <summary>Gets or sets the remarks/notes (備註).</summary>
        [JsonPropertyName("備註")]
        public string 備註 { get; set; } = string.Empty;
    }

    /// <summary>
    /// API response wrapper containing paged asset data array and total matching record count.
    /// </summary>
    public class AssetsResponse
    {
        /// <summary>Gets or sets the list of asset DTOs matching the query for the requested page.</summary>
        [JsonPropertyName("data")]
        public List<AssetDto> Data { get; set; } = new();

        /// <summary>Gets or sets the total count of matching asset records before pagination.</summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    /// <summary>
    /// Data Transfer Object representing a custodian search record.
    /// </summary>
    public class CustodianDto
    {
        [JsonPropertyName("保管人")]
        public string 保管人 { get; set; } = string.Empty;

        [JsonPropertyName("姓名")]
        public string 姓名 { get; set; } = string.Empty;

        [JsonPropertyName("保管代號")]
        public string 保管代號 { get; set; } = string.Empty;

        [JsonPropertyName("保管人部門")]
        public string 保管人部門 { get; set; } = string.Empty;

        [JsonPropertyName("isBookmarked")]
        public bool IsBookmarked { get; set; }
    }

    /// <summary>
    /// Data Transfer Object representing a department search record.
    /// </summary>
    public class DepartmentDto
    {
        [JsonPropertyName("保管代號")]
        public string 保管代號 { get; set; } = string.Empty;

        [JsonPropertyName("保管人部門")]
        public string 保管人部門 { get; set; } = string.Empty;

        [JsonPropertyName("isBookmarked")]
        public bool IsBookmarked { get; set; }
    }

    /// <summary>
    /// Request model for toggling a bookmark.
    /// </summary>
    public class ToggleBookmarkRequest
    {
        [JsonPropertyName("custodianCode")]
        public string CustodianCode { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for editing asset custody info.
    /// </summary>
    public class EditAssetRequest
    {
        [JsonPropertyName("保管人")]
        public string 保管人 { get; set; } = string.Empty;

        [JsonPropertyName("保管代號")]
        public string 保管代號 { get; set; } = string.Empty;

        [JsonPropertyName("放置地點")]
        public string 放置地點 { get; set; } = string.Empty;

        [JsonPropertyName("備註")]
        public string 備註 { get; set; } = string.Empty;
    }

    internal class FilteredAssetRecord
    {
        public Astmb Astmb { get; set; } = null!;
        public Astmc Astmc { get; set; } = null!;
        public Cmsme? Cmsme { get; set; }
        public Cmsmv? Cmsmv { get; set; }
    }

    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IniSettingsService _settingsService;

        public AssetsController(AppDbContext context, IniSettingsService settingsService)
        {
            _context = context;
            _settingsService = settingsService;
        }

        private IQueryable<FilteredAssetRecord> GetFilteredAssetQuery(string? managerType, string? assetId)
        {
            var baseQuery = from a in _context.Astmbs
                            join b in _context.Astmcs on a.Mb001 equals b.Mc001
                            join c in _context.Cmsmes on b.Mc002 equals c.Me001 into bc
                            from c in bc.DefaultIfEmpty()
                            join d in _context.Cmsmvs on b.Mc003 equals d.Mv001 into bd
                            from d in bd.DefaultIfEmpty()
                            where b.Mc004 > 0
                               && a.Mb013 != null && a.Mb013 != ""
                               && a.Mb017 == ""
                               && a.Mb013 != "P" && a.Mb013 != "A"
                               && a.Mb039 == "Y"
                            select new FilteredAssetRecord
                            {
                                Astmb = a,
                                Astmc = b,
                                Cmsme = c,
                                Cmsmv = d
                            };

            if (!string.IsNullOrWhiteSpace(managerType))
            {
                string mgr = managerType.Trim().ToUpper();
                baseQuery = baseQuery.Where(x => x.Astmb.Mb013 != null && x.Astmb.Mb013.ToUpper() == mgr);
            }

            if (!string.IsNullOrWhiteSpace(assetId))
            {
                string term = assetId.Trim();
                bool hasWildcards = term.Contains('?') || term.Contains('_') || term.Contains('*') || term.Contains('%');

                string pattern = (hasWildcards
                    ? term.Replace('?', '_').Replace('*', '%')
                    : $"%{term}%").ToLower();

                baseQuery = baseQuery.Where(x => EF.Functions.Like(x.Astmb.Mb001.Trim().ToLower(), pattern));
            }

            return baseQuery;
        }

        private async Task<List<AssetDto>> FetchAssetDtosAsync(IQueryable<FilteredAssetRecord> baseQuery, int? skip = null, int? take = null)
        {
            IQueryable<FilteredAssetRecord> queryToFetch = baseQuery.OrderBy(x => x.Astmb.Mb001);

            if (skip.HasValue)
            {
                queryToFetch = queryToFetch.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                queryToFetch = queryToFetch.Take(take.Value);
            }

            var rawItems = await queryToFetch
                .Select(x => new
                {
                    Mb001 = x.Astmb.Mb001,
                    Mb002 = x.Astmb.Mb002,
                    Mb003 = x.Astmb.Mb003,
                    Mc003 = x.Astmc.Mc003,
                    Mv002 = x.Cmsmv != null ? x.Cmsmv.Mv002 : null,
                    Mc002 = x.Astmc.Mc002,
                    Me002 = x.Cmsme != null ? x.Cmsme.Me002 : null,
                    Mc006 = x.Astmc.Mc006,
                    Mb007 = x.Astmb.Mb007,
                    Mb008 = x.Astmb.Mb008,
                    Mb013 = x.Astmb.Mb013,
                    Mc005 = x.Astmc.Mc005
                })
                .ToListAsync();

            return rawItems.Select(x => new AssetDto
            {
                資產編號 = x.Mb001?.Trim() ?? "",
                資產名稱 = x.Mb002?.Trim() ?? "",
                資產規格 = x.Mb003?.Trim() ?? "",
                保管人 = x.Mc003?.Trim() ?? "",
                姓名 = x.Mv002?.Trim() ?? "",
                保管代號 = x.Mc002?.Trim() ?? "",
                保管人部門 = x.Me002?.Trim() ?? "",
                放置地點 = x.Mc006?.Trim() ?? "",
                供應廠商 = x.Mb007?.Trim() ?? "",
                供應商簡稱 = x.Mb008?.Trim() ?? "",
                管理區分 = x.Mb013?.Trim() ?? "",
                備註 = x.Mc005?.Trim() ?? ""
            }).ToList();
        }

        [HttpGet("assets")]
        [HttpGet("api/assets")]
        public async Task<IActionResult> GetAssets(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? managerType = null,
            [FromQuery] string? assetId = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            int offset = (page - 1) * pageSize;

            var baseQuery = GetFilteredAssetQuery(managerType, assetId);

            int total = await baseQuery.CountAsync();
            var data = await FetchAssetDtosAsync(baseQuery, offset, pageSize);

            return Ok(new AssetsResponse
            {
                Data = data,
                Total = total
            });
        }

        [HttpGet("export")]
        [HttpGet("api/export")]
        public async Task<IActionResult> Export(
            [FromQuery] string? managerType = null,
            [FromQuery] string? assetId = null)
        {
            var baseQuery = GetFilteredAssetQuery(managerType, assetId);
            var data = await FetchAssetDtosAsync(baseQuery);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("資產");

            string[] headers = new[]
            {
                "資產編號", "資產名稱", "資產規格", "保管人", "姓名", "保管代號",
                "保管人部門", "放置地點", "供應廠商", "供應商簡稱", "管理區分", "備註"
            };

            for (int col = 0; col < headers.Length; col++)
            {
                worksheet.Cell(1, col + 1).Value = headers[col];
            }

            for (int row = 0; row < data.Count; row++)
            {
                var item = data[row];
                worksheet.Cell(row + 2, 1).Value = item.資產編號;
                worksheet.Cell(row + 2, 2).Value = item.資產名稱;
                worksheet.Cell(row + 2, 3).Value = item.資產規格;
                worksheet.Cell(row + 2, 4).Value = item.保管人;
                worksheet.Cell(row + 2, 5).Value = item.姓名;
                worksheet.Cell(row + 2, 6).Value = item.保管代號;
                worksheet.Cell(row + 2, 7).Value = item.保管人部門;
                worksheet.Cell(row + 2, 8).Value = item.放置地點;
                worksheet.Cell(row + 2, 9).Value = item.供應廠商;
                worksheet.Cell(row + 2, 10).Value = item.供應商簡稱;
                worksheet.Cell(row + 2, 11).Value = item.管理區分;
                worksheet.Cell(row + 2, 12).Value = item.備註;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "assets.xlsx");
        }

        [HttpGet("api/bookmarks")]
        public IActionResult GetBookmarks()
        {
            var bookmarks = _settingsService.GetBookmarkedCustodians();
            return Ok(bookmarks.ToList());
        }

        [HttpPost("api/bookmarks/toggle")]
        public IActionResult ToggleBookmark([FromBody] ToggleBookmarkRequest req)
        {
            if (string.IsNullOrWhiteSpace(req?.CustodianCode))
            {
                return BadRequest("Custodian code cannot be empty.");
            }
            bool isBookmarked = _settingsService.ToggleCustodianBookmark(req.CustodianCode);
            var allBookmarks = _settingsService.GetBookmarkedCustodians();
            return Ok(new
            {
                custodianCode = req.CustodianCode.Trim(),
                isBookmarked,
                bookmarks = allBookmarks.ToList()
            });
        }

        [HttpGet("api/bookmarks/departments")]
        public IActionResult GetDepartmentBookmarks()
        {
            var bookmarks = _settingsService.GetBookmarkedDepartments();
            return Ok(bookmarks.ToList());
        }

        [HttpPost("api/bookmarks/toggle-dept")]
        public IActionResult ToggleDepartmentBookmark([FromBody] ToggleBookmarkRequest req)
        {
            if (string.IsNullOrWhiteSpace(req?.CustodianCode))
            {
                return BadRequest("Department code cannot be empty.");
            }
            bool isBookmarked = _settingsService.ToggleDepartmentBookmark(req.CustodianCode);
            var allBookmarks = _settingsService.GetBookmarkedDepartments();
            return Ok(new
            {
                departmentCode = req.CustodianCode.Trim(),
                isBookmarked,
                bookmarks = allBookmarks.ToList()
            });
        }

        [HttpGet("api/custodians")]
        public async Task<IActionResult> GetCustodians(
            [FromQuery] string? q = null,
            [FromQuery] string? deptCode = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var bookmarks = _settingsService.GetBookmarkedCustodians();
            q = q?.Trim();
            deptCode = deptCode?.Trim();

            List<CustodianDto> responseList = new();

            // Always fetch ALL bookmarked custodians on page 1 and pin them to top regardless of search query
            if (page == 1 && bookmarks.Count > 0)
            {
                var bookmarkedList = await (from d in _context.Cmsmvs
                                            join c in _context.Cmsmes on d.Mv004 equals c.Me001 into dc
                                            from c in dc.DefaultIfEmpty()
                                            where bookmarks.Contains(d.Mv001.Trim())
                                            orderby d.Mv001
                                            select new CustodianDto
                                            {
                                                保管人 = d.Mv001.Trim(),
                                                姓名 = d.Mv002 != null ? d.Mv002.Trim() : "",
                                                保管代號 = d.Mv004 != null ? d.Mv004.Trim() : "",
                                                保管人部門 = c != null && c.Me002 != null ? c.Me002.Trim() : "",
                                                IsBookmarked = true
                                            }).ToListAsync();

                responseList.AddRange(bookmarkedList);
            }

            // Query matching database custodians
            var baseQuery = from d in _context.Cmsmvs
                            join c in _context.Cmsmes on d.Mv004 equals c.Me001 into dc
                            from c in dc.DefaultIfEmpty()
                            select new { d, c };

            if (!string.IsNullOrEmpty(q))
            {
                bool hasWildcards = q.Contains('?') || q.Contains('_') || q.Contains('*') || q.Contains('%');
                string pattern = (hasWildcards
                    ? q.Replace('?', '_').Replace('*', '%')
                    : $"%{q}%").ToLower();

                baseQuery = baseQuery.Where(x => EF.Functions.Like(x.d.Mv001.Trim().ToLower(), pattern) ||
                                                 (x.d.Mv002 != null && EF.Functions.Like(x.d.Mv002.Trim().ToLower(), pattern)));
            }
            else if (!string.IsNullOrEmpty(deptCode))
            {
                baseQuery = baseQuery.Where(x => x.d.Mv004 != null && x.d.Mv004.Trim() == deptCode);
            }

            int offset = (page - 1) * pageSize;
            var pageItems = await baseQuery.OrderBy(x => x.d.Mv001).Skip(offset).Take(pageSize).ToListAsync();

            List<CustodianDto> searchDtos = pageItems.Select(item => new CustodianDto
            {
                保管人 = item.d.Mv001?.Trim() ?? "",
                姓名 = item.d.Mv002?.Trim() ?? "",
                保管代號 = item.d.Mv004?.Trim() ?? "",
                保管人部門 = item.c?.Me002?.Trim() ?? "",
                IsBookmarked = bookmarks.Contains(item.d.Mv001?.Trim() ?? "")
            }).ToList();

            // Fallback rule if search results on page 1 are <= 1
            if (page == 1 && searchDtos.Count <= 1 && !string.IsNullOrEmpty(q))
            {
                string cleanTerm = q.Replace("*", "").Replace("%", "").Replace("?", "").Replace("_", "").Trim();
                var fallbackItems = await (from d in _context.Cmsmvs
                                           join c in _context.Cmsmes on d.Mv004 equals c.Me001 into dc
                                           from c in dc.DefaultIfEmpty()
                                           where d.Mv001.Trim().CompareTo(cleanTerm) >= 0
                                           orderby d.Mv001
                                           select new { d, c }).Take(10).ToListAsync();

                foreach (var item in fallbackItems)
                {
                    string code = item.d.Mv001?.Trim() ?? "";
                    if (!searchDtos.Any(s => s.保管人.Equals(code, StringComparison.OrdinalIgnoreCase)))
                    {
                        searchDtos.Add(new CustodianDto
                        {
                            保管人 = code,
                            姓名 = item.d.Mv002?.Trim() ?? "",
                            保管代號 = item.d.Mv004?.Trim() ?? "",
                            保管人部門 = item.c?.Me002?.Trim() ?? "",
                            IsBookmarked = bookmarks.Contains(code)
                        });
                    }
                }
            }

            // Append searchDtos to responseList, ensuring no duplicate custodian codes
            foreach (var item in searchDtos)
            {
                if (!responseList.Any(r => r.保管人.Equals(item.保管人, StringComparison.OrdinalIgnoreCase)))
                {
                    responseList.Add(item);
                }
            }

            return Ok(responseList);
        }

        [HttpGet("api/custodians/details/{code}")]
        public async Task<IActionResult> GetCustodianDetail(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return NotFound();

            string cleanCode = code.Trim();
            var item = await (from d in _context.Cmsmvs
                              join c in _context.Cmsmes on d.Mv004 equals c.Me001 into dc
                              from c in dc.DefaultIfEmpty()
                              where d.Mv001.Trim() == cleanCode
                              select new CustodianDto
                              {
                                  保管人 = d.Mv001.Trim(),
                                  姓名 = d.Mv002 != null ? d.Mv002.Trim() : "",
                                  保管代號 = d.Mv004 != null ? d.Mv004.Trim() : "",
                                  保管人部門 = c != null && c.Me002 != null ? c.Me002.Trim() : ""
                              }).FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound(new { message = "Custodian not found" });
            }

            return Ok(item);
        }

        [HttpGet("api/departments")]
        public async Task<IActionResult> GetDepartments(
            [FromQuery] string? q = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var bookmarks = _settingsService.GetBookmarkedDepartments();
            q = q?.Trim();

            List<DepartmentDto> responseList = new();

            // Always fetch ALL bookmarked departments on page 1 and pin to top
            if (page == 1 && bookmarks.Count > 0)
            {
                var bookmarkedDepts = await _context.Cmsmes
                    .Where(c => bookmarks.Contains(c.Me001.Trim()))
                    .OrderBy(c => c.Me001)
                    .Select(c => new DepartmentDto
                    {
                        保管代號 = c.Me001.Trim(),
                        保管人部門 = c.Me002 != null ? c.Me002.Trim() : "",
                        IsBookmarked = true
                    }).ToListAsync();

                responseList.AddRange(bookmarkedDepts);
            }

            IQueryable<Cmsme> query = _context.Cmsmes;

            if (!string.IsNullOrEmpty(q))
            {
                bool hasWildcards = q.Contains('?') || q.Contains('_') || q.Contains('*') || q.Contains('%');
                string pattern = (hasWildcards
                    ? q.Replace('?', '_').Replace('*', '%')
                    : $"%{q}%").ToLower();

                query = query.Where(c => EF.Functions.Like(c.Me001.Trim().ToLower(), pattern) ||
                                         (c.Me002 != null && EF.Functions.Like(c.Me002.Trim().ToLower(), pattern)));
            }

            int offset = (page - 1) * pageSize;
            var pageItems = await query.OrderBy(c => c.Me001).Skip(offset).Take(pageSize).ToListAsync();

            var searchDtos = pageItems.Select(c => new DepartmentDto
            {
                保管代號 = c.Me001.Trim(),
                保管人部門 = c.Me002?.Trim() ?? "",
                IsBookmarked = bookmarks.Contains(c.Me001.Trim())
            }).ToList();

            foreach (var item in searchDtos)
            {
                if (!responseList.Any(r => r.保管代號.Equals(item.保管代號, StringComparison.OrdinalIgnoreCase)))
                {
                    responseList.Add(item);
                }
            }

            return Ok(responseList);
        }

        [HttpPut("assets/{id}")]
        [HttpPut("api/assets/{id}")]
        public async Task<IActionResult> UpdateAsset(string id, [FromBody] EditAssetRequest req)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest("Asset ID is required.");

            string assetId = id.Trim();
            string custodianCode = req.保管人?.Trim() ?? "";
            string deptCode = req.保管代號?.Trim() ?? "";

            // Validation 1: 保管人 must match values from database (CMSMV)
            bool custodianValid = await _context.Cmsmvs.AnyAsync(x => x.Mv001.Trim() == custodianCode);
            if (!custodianValid)
            {
                return BadRequest(new { message = $"保管人 ({custodianCode}) 在資料庫中不存在" });
            }

            // Validation 2: 保管代號 must match values from database (CMSME)
            bool deptValid = await _context.Cmsmes.AnyAsync(x => x.Me001.Trim() == deptCode);
            if (!deptValid)
            {
                return BadRequest(new { message = $"保管代號 ({deptCode}) 在資料庫中不存在" });
            }

            string location = req.放置地點?.Trim() ?? "";
            string remark = req.備註?.Trim() ?? "";

            // Write ONLY the minimum changes (MC002, MC003, MC005, MC006 in ASTMC table)
            int rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE ASTMC SET MC002 = {deptCode}, MC003 = {custodianCode}, MC005 = {remark}, MC006 = {location} WHERE TRIM(MC001) = {assetId}");

            if (rowsAffected == 0)
            {
                return NotFound(new { message = $"找不到資產編號 ({assetId}) 的保管記錄" });
            }

            return Ok(new { success = true, message = "更新成功" });
        }
    }
}
