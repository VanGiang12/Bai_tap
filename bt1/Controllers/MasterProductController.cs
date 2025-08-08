using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.ComponentModel;
using System.Net.Http;
using web_app.Models;

namespace web_app.Controllers
{
    public class MasterProductController : Controller
    {
        private readonly HttpClient _httpClient;
        public MasterProductController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<MasterProduct>> MasterProducts()
        {
            var response = await _httpClient.GetAsync("https://localhost:44343/api/Products");
            var json = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<MasterProduct>>(json);

            return products;
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var response = await _httpClient.GetAsync("https://localhost:44343/api/Products");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }
            var json = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<MasterProduct>>(json);
            return Json(products);
        }

        public async Task<IActionResult> Index(string categoryId, string searchValue, string totalPage, int page = 1)
        {
            var products = await MasterProducts();

            if (!string.IsNullOrEmpty(categoryId) && !string.IsNullOrEmpty(searchValue))
            {
                var prop = typeof(MasterProduct).GetProperty(categoryId);
                if (prop != null)
                {
                    products = products
                        .Where(p => prop.GetValue(p)?.ToString()?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) == true)
                        .ToList();
                }
            }

            int totalRecords = products.Count;

            int pageSize = -1;  
            if (totalPage == "-1" || totalPage==null)
            {
                pageSize = totalRecords; 
            }
            else if (int.TryParse(totalPage, out int parsedPageSize) && parsedPageSize > 0)
            {
                pageSize = parsedPageSize;
            }

            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            if (totalPage != "-1") 
            {
                products = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }

            var propertyNames = typeof(MasterProduct).GetProperties().Select(p => p.Name).ToList();
            ViewBag.Categories = propertyNames.Select(p => new SelectListItem
            {
                Text = p,
                Value = p,
                Selected = (p == categoryId)
            }).ToList();

            ViewBag.SelectedTotalPage = totalPage ?? "Toàn bộ"; 
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            ViewBag.CategoryId = categoryId;
            ViewBag.SearchValue = searchValue;

            return View(products);
        }
        [HttpPost]
        public async Task<IActionResult> Insert(MasterProduct model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44343/");
                var response = await client.PostAsJsonAsync("api/Products", model);

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(MasterProduct model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44343/");
                var response = await client.PutAsJsonAsync($"api/Products/{model.Id}", model);

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44343/");
                var response = await client.DeleteAsync($"api/Products/{id}");

            }

            return RedirectToAction("Index");
        }

        public IActionResult DownloadTemplate()
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");

                worksheet.Cells[1, 1].Value = "Mã Sản Phẩm";
                worksheet.Cells[1, 2].Value = "Tên Sản Phẩm";
                worksheet.Cells[1, 3].Value = "Đơn Vị Tính";
                worksheet.Cells[1, 4].Value = "Quy Cách";
                worksheet.Cells[1, 5].Value = "Số Lượng/Thùng";
                worksheet.Cells[1, 6].Value = "Trọng Lượng";

                worksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;
                worksheet.Cells.AutoFitColumns();

                package.Save();
            }

            stream.Position = 0;
            string excelName = $"FileMau_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile uploadFile)
        {
            if (uploadFile == null || uploadFile.Length == 0)
            {
                return BadRequest("Vui lòng chọn tệp Excel.");
            }

            var errors = new List<string>();
            var validProducts = new List<MasterProduct>();
            var products = await MasterProducts();

            var existingCodes = products.Select(p => p.ProductCode).ToHashSet();

            using (var stream = new MemoryStream())
            {
                await uploadFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        errors.Add("File không chứa sheet hợp lệ.");
                        return Json(new { success = false, errors });
                    }

                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string code = worksheet.Cells[row, 1]?.Text?.Trim();
                        string name = worksheet.Cells[row, 2]?.Text?.Trim();
                        string unit = worksheet.Cells[row, 3]?.Text?.Trim();
                        string spec = worksheet.Cells[row, 4]?.Text?.Trim();
                        string quantityStr = worksheet.Cells[row, 5]?.Text?.Trim();
                        string weightStr = worksheet.Cells[row, 6]?.Text?.Trim();


                        if (string.IsNullOrEmpty(code)) errors.Add($"Dòng {row}: Mã sản phẩm không được để trống");
                        if (string.IsNullOrEmpty(name)) errors.Add($"Dòng {row}: Tên sản phẩm không được để trống");
                        if (string.IsNullOrEmpty(unit)) errors.Add($"Dòng {row}: Đơn vị tính không được để trống");
                        if (string.IsNullOrEmpty(spec)) errors.Add($"Dòng {row}: Quy cách không được để trống");
                        if (string.IsNullOrEmpty(quantityStr)) errors.Add($"Dòng {row}: Số lượng/Thùng không được để trống");
                        if (string.IsNullOrEmpty(weightStr)) errors.Add($"Dòng {row}: Trọng lượng không được để trống");


                        if (!string.IsNullOrEmpty(code) && existingCodes.Contains(code))
                        {
                            errors.Add($"Dòng {row}: Mã sản phẩm '{code}' đã có trên hệ thống");
                            continue;
                        }


                        if (!errors.Any(e => e.Contains($"Dòng {row}:")))
                        {
                            validProducts.Add(new MasterProduct(code, name, unit, spec,
                                int.TryParse(quantityStr, out int q) ? q : 0,
                                decimal.TryParse(weightStr, out decimal w) ? w : 0));
                        }
                    }
                }
            }

            if (errors.Any())
            {
                return Json(new { success = false, errors });
            }


            var response = await _httpClient.PostAsJsonAsync("https://localhost:44343/api/products/upload", validProducts);

            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, errors = new List<string> { "Gửi dữ liệu sang API thất bại." } });
            }

            return Json(new { success = true });
        }

    }
}
