using System.Data;
using System.Text.Json.Serialization;
using ClosedXML.Excel;
using LegacyWebBridge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegacyWebBridge.Controllers
{
    public class AssetDto
    {
        [JsonPropertyName("資產編號")]
        public string 資產編號 { get; set; } = string.Empty;

        [JsonPropertyName("資產名稱")]
        public string 資產名稱 { get; set; } = string.Empty;

        [JsonPropertyName("資產規格")]
        public string 資產規格 { get; set; } = string.Empty;

        [JsonPropertyName("保管人")]
        public string 保管人 { get; set; } = string.Empty;

        [JsonPropertyName("姓名")]
        public string 姓名 { get; set; } = string.Empty;

        [JsonPropertyName("保管代號")]
        public string 保管代號 { get; set; } = string.Empty;

        [JsonPropertyName("保管人部門")]
        public string 保管人部門 { get; set; } = string.Empty;

        [JsonPropertyName("放置地點")]
        public string 放置地點 { get; set; } = string.Empty;

        [JsonPropertyName("供應廠商")]
        public string 供應廠商 { get; set; } = string.Empty;

        [JsonPropertyName("供應商簡稱")]
        public string 供應商簡稱 { get; set; } = string.Empty;

        [JsonPropertyName("管理區分")]
        public string 管理區分 { get; set; } = string.Empty;

        [JsonPropertyName("備註")]
        public string 備註 { get; set; } = string.Empty;
    }

    public class AssetsResponse
    {
        [JsonPropertyName("data")]
        public List<AssetDto> Data { get; set; } = new();

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AssetsController(AppDbContext context)
        {
            _context = context;
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
                            select new { a, b, c, d };

            if (!string.IsNullOrWhiteSpace(managerType))
            {
                baseQuery = baseQuery.Where(x => x.a.Mb013 == managerType);
            }

            if (!string.IsNullOrWhiteSpace(assetId))
            {
                baseQuery = baseQuery.Where(x => x.a.Mb001.Contains(assetId));
            }

            int total = await baseQuery.CountAsync();

            var rawItems = await baseQuery
                .OrderBy(x => x.a.Mb001)
                .Skip(offset)
                .Take(pageSize)
                .Select(x => new
                {
                    Mb001 = x.a.Mb001,
                    Mb002 = x.a.Mb002,
                    Mb003 = x.a.Mb003,
                    Mc003 = x.b.Mc003,
                    Mv002 = x.d != null ? x.d.Mv002 : null,
                    Mc002 = x.b.Mc002,
                    Me002 = x.c != null ? x.c.Me002 : null,
                    Mc006 = x.b.Mc006,
                    Mb007 = x.a.Mb007,
                    Mb008 = x.a.Mb008,
                    Mb013 = x.a.Mb013,
                    Mc005 = x.b.Mc005
                })
                .ToListAsync();

            var data = rawItems.Select(x => new AssetDto
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
                            select new { a, b, c, d };

            if (!string.IsNullOrWhiteSpace(managerType))
            {
                baseQuery = baseQuery.Where(x => x.a.Mb013 == managerType);
            }

            if (!string.IsNullOrWhiteSpace(assetId))
            {
                baseQuery = baseQuery.Where(x => x.a.Mb001.Contains(assetId));
            }

            var rawItems = await baseQuery
                .OrderBy(x => x.a.Mb001)
                .Select(x => new
                {
                    Mb001 = x.a.Mb001,
                    Mb002 = x.a.Mb002,
                    Mb003 = x.a.Mb003,
                    Mc003 = x.b.Mc003,
                    Mv002 = x.d != null ? x.d.Mv002 : null,
                    Mc002 = x.b.Mc002,
                    Me002 = x.c != null ? x.c.Me002 : null,
                    Mc006 = x.b.Mc006,
                    Mb007 = x.a.Mb007,
                    Mb008 = x.a.Mb008,
                    Mb013 = x.a.Mb013,
                    Mc005 = x.b.Mc005
                })
                .ToListAsync();

            var data = rawItems.Select(x => new AssetDto
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
    }
}
