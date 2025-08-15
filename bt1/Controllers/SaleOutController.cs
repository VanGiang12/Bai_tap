using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OfficeOpenXml;
using web_app.Model;
using web_app.Models;

namespace web_app.Controllers
{
    public class SaleOutController : Controller
    {
        private readonly HttpClient _httpClient;
        public SaleOutController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<SaleOutDTO>> SaleOuts()
        {
            var response = await _httpClient.GetAsync("https://localhost:44343/api/SaleOuts");
            var json = await response.Content.ReadAsStringAsync();
            var saleOuts = JsonConvert.DeserializeObject<List<SaleOutDTO>>(json);

            return saleOuts;
        }

        public async Task<List<MasterProduct>> MasterProducts()
        {
            var response = await _httpClient.GetAsync("https://localhost:44343/api/Products");
            var json = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<MasterProduct>>(json);

            return products;
        }

        public async Task<IActionResult> Index(string categoryId, string searchValue, string totalPage, int page = 1)
        {
            var saleOuts = await SaleOuts();

            if (!string.IsNullOrEmpty(categoryId) && !string.IsNullOrEmpty(searchValue))
            {
                switch (categoryId)
                {
                    case "ProductName":
                        saleOuts = saleOuts.Where(p => p.ProductName?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) == true).ToList();
                        break;
                    case "ProductCode":
                        saleOuts = saleOuts.Where(p => p.ProductCode?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) == true).ToList();
                        break;
                    default:
                        var prop = typeof(SaleOutDTO).GetProperty(categoryId);
                        if (prop != null)
                        {
                            saleOuts = saleOuts
                                .Where(p => prop.GetValue(p)?.ToString()?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) == true)
                                .ToList();
                        }
                        break;
                }

            }
            int totalRecords = saleOuts.Count;

            int pageSize = -1;
            if (totalPage == "-1" || totalPage == null)
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
                saleOuts = saleOuts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }

            ViewBag.Categories = new List<SelectListItem>
            {
                new SelectListItem { Text = "Số PO khách hàng", Value = "CustomerPoNo", Selected = (categoryId == "CustomerPoNo") },
                new SelectListItem { Text = "Khách hàng", Value = "CustomerName", Selected = (categoryId == "CustomerName") },
                new SelectListItem { Text = "Ngày đặt hàng", Value = "OrderDate", Selected = (categoryId == "OrderDate") },
                new SelectListItem { Text = "Mã sản phẩm", Value = "ProductCode", Selected = (categoryId == "ProductCode") },
                new SelectListItem { Text = "Tên sản phẩm", Value = "ProductName", Selected = (categoryId == "ProductName") }
            };
            ViewBag.SelectedTotalPage = totalPage ?? "Toàn bộ";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            var customerName = saleOuts.Select(x => x.CustomerName).Distinct()
                                        .Select(name => new SelectListItem
                                        {
                                            Text = name,
                                            Value = name
                                        }).ToList();

            ViewBag.CustomerName = customerName;

            var response = await _httpClient.GetAsync("https://localhost:44343/api/Products");
            var json = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<MasterProduct>>(json);

            ViewBag.ProductCodes = products.Select(p => new SelectListItem
            {
                Text = p.ProductCode + " - " + p.ProductName,
                Value = p.ProductCode
            }).ToList();

            ViewBag.AllProducts = products;

            return View(saleOuts);
        }
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] SaleOut model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44343/");
                var response = await client.PostAsJsonAsync("api/SaleOuts", model);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Thêm thành công!" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = "Lỗi khi thêm sản phẩm!", details = errorContent });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaleOut model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44343/");
                var response = await client.PutAsJsonAsync($"api/SaleOuts/{model.Id}", model);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Sửa thành công!" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { success = false, message = "Lỗi khi Sửa sản phẩm!", details = errorContent });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44343/");
                var response = await client.DeleteAsync($"api/SaleOuts/{id}");
            }

            return RedirectToAction("Index");
        }

        public IActionResult DownloadTemplate()
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("SaleOut");

                worksheet.Cells[1, 1].Value = "Số PO khách hàng";
                worksheet.Cells[1, 2].Value = "Ngày đặt hàng";
                worksheet.Cells[1, 3].Value = "Khách hàng";
                worksheet.Cells[1, 4].Value = "Mã sản phẩm";
                worksheet.Cells[1, 5].Value = "Đơn vị tính";
                worksheet.Cells[1, 6].Value = "Đơn giá";
                worksheet.Cells[1, 7].Value = "Số lượng";
                worksheet.Cells[1, 8].Value = "Số lượng/thùng";
                worksheet.Cells[1, 1, 1, 9].Style.Font.Bold = true;
                worksheet.Column(2).Style.Numberformat.Format = "yyyy-MM-dd";
                worksheet.Cells.AutoFitColumns();

                package.Save();
            }

            stream.Position = 0;
            string excelName = $"SaleOut_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile uploadFile)
        {
            if (uploadFile == null || uploadFile.Length == 0)
            {
                return Json(new { success = false, errors = new List<string> { "Vui lòng chọn tệp Excel." } });
            }

            var errors = new List<string>();
            var validSaleOuts = new List<SaleOut>();

            var existingSaleOuts = await SaleOuts();
            var existingKeys = existingSaleOuts
                .Select(s => $"{s.CustomerPoNo}__{s.ProductCode}")
                .ToHashSet();

            var masterProducts = await MasterProducts();
            var productDict = masterProducts.ToDictionary(p => p.ProductCode, p => p);

            using (var stream = new MemoryStream())
            {
                await uploadFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return Json(new { success = false, errors = new List<string> { "File không chứa sheet hợp lệ." } });
                    }

                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string poNo = worksheet.Cells[row, 1]?.Text?.Trim();
                        object orderDateRaw = worksheet.Cells[row, 2]?.Value;
                        string customerName = worksheet.Cells[row, 3]?.Text?.Trim();
                        string productCode = worksheet.Cells[row, 4]?.Text?.Trim();
                        string unit = worksheet.Cells[row, 5]?.Text?.Trim();
                        string priceStr = worksheet.Cells[row, 6]?.Text?.Trim();
                        string quantityStr = worksheet.Cells[row, 7]?.Text?.Trim();
                        string quantityPerBoxStr = worksheet.Cells[row, 8]?.Text?.Trim();

                        if (string.IsNullOrEmpty(poNo)) errors.Add($"Dòng {row}: Số PO khách hàng không được để trống");
                        if (orderDateRaw == null || string.IsNullOrEmpty(orderDateRaw.ToString())) errors.Add($"Dòng {row}: Ngày đặt hàng không được để trống");
                        if (string.IsNullOrEmpty(customerName)) errors.Add($"Dòng {row}: Khách hàng không được để trống");
                        if (string.IsNullOrEmpty(productCode)) errors.Add($"Dòng {row}: Mã sản phẩm không được để trống");
                        if (string.IsNullOrEmpty(unit)) errors.Add($"Dòng {row}: Đơn vị tính không được để trống");
                        if (string.IsNullOrEmpty(priceStr)) errors.Add($"Dòng {row}: đơn giá không được để trống");
                        if (string.IsNullOrEmpty(quantityStr)) errors.Add($"Dòng {row}: Số lượng không được để trống");
                        if (string.IsNullOrEmpty(quantityPerBoxStr)) errors.Add($"Dòng {row}: Số lượng/thùng không được để trống");

                        if (errors.Any(e => e.Contains($"Dòng {row}:"))) continue;

                        var key = $"{poNo}__{productCode}";
                        if (existingKeys.Contains(key))
                        {
                            errors.Add($"Dòng {row}: Số PO khách hàng '{poNo}'; Mã sản phẩm '{productCode}' đã có trên hệ thống");
                            continue;
                        }

                        int orderDateInt;
                        try
                        {
                            DateTime dateValue;
                            if (orderDateRaw is double oaDate)
                            {
                                dateValue = DateTime.FromOADate(oaDate);
                            }
                            else if (DateTime.TryParse(orderDateRaw.ToString(), out var parsedDate))
                            {
                                dateValue = parsedDate;
                            }
                            else
                            {
                                errors.Add($"Dòng {row}: Ngày đặt hàng không hợp lệ");
                                continue;
                            }
                            orderDateInt = int.Parse(dateValue.ToString("yyyyMMdd"));
                        }
                        catch
                        {
                            errors.Add($"Dòng {row}: Ngày đặt hàng không hợp lệ");
                            continue;
                        }

                        if (!decimal.TryParse(quantityStr, out var quantity))
                        {
                            errors.Add($"Dòng {row}: Số lượng không hợp lệ");
                            continue;
                        }
                        if (!decimal.TryParse(quantityPerBoxStr, out var quantityPerBox))
                        {
                            errors.Add($"Dòng {row}: Số lượng/thùng không hợp lệ");
                            continue;
                        }
                        if (!decimal.TryParse(priceStr, out var price))
                        {
                            errors.Add($"Dòng {row}: Đơn giá không hợp lệ");
                            continue;
                        }

                        if (!productDict.TryGetValue(productCode, out var productInfo))
                        {
                            errors.Add($"Dòng {row}: Mã sản phẩm '{productCode}' không tồn tại trong Master Sản phẩm");
                            continue;
                        }

                        decimal boxQuantity = quantityPerBox != 0 ? quantity / quantityPerBox : 0;

                        validSaleOuts.Add(new SaleOut
                        {
                            CustomerPoNo = poNo,
                            OrderDate = orderDateInt,
                            CustomerName = customerName,
                            Quantity = quantity,
                            QuantityPerBox = quantityPerBox,
                            BoxQuantity = boxQuantity,
                            ProductId = productInfo.Id,
                            Price = price,
                            Amount = price * quantity
                        });
                    }
                }
            }

            if (errors.Any())
            {
                return Json(new { success = false, errors });
            }

            var response = await _httpClient.PostAsJsonAsync("https://localhost:44343/api/saleouts/upload", validSaleOuts);

            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, errors = new List<string> { "Gửi dữ liệu sang API thất bại." } });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DownloadSaleOutReport(DateTime startDate, DateTime endDate)
        {
 
            int start = int.Parse(startDate.ToString("yyyyMMdd"));
            int end = int.Parse(endDate.ToString("yyyyMMdd"));
            Console.WriteLine("start - " + start);
            var url = $"https://localhost:44343/api/SaleOuts/saleout-report?startDate={start}&endDate={end}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Lỗi khi lấy dữ liệu từ API báo cáo.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var reportData = JsonConvert.DeserializeObject<List<SaleOutReport>>(json);

            if (reportData == null)
            {
                return BadRequest("Dữ liệu báo cáo rỗng.");
            }

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Báo cáo tổng hợp");

                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Mã sản phẩm";
                worksheet.Cells[1, 3].Value = "Tên sản phẩm";
                worksheet.Cells[1, 4].Value = "Số lượng";
                worksheet.Cells[1, 5].Value = "Đơn giá";
                worksheet.Cells[1, 6].Value = "Thành tiền";

                int row = 2;
                int count = 1;
                foreach (var item in reportData)
                {
                    worksheet.Cells[row, 1].Value = count;
                    worksheet.Cells[row, 2].Value = item.ProductCode;
                    worksheet.Cells[row, 3].Value = item.ProductName;
                    worksheet.Cells[row, 4].Value = item.TotalQuantity;
                    worksheet.Cells[row, 5].Value = item.UnitPrice;
                    worksheet.Cells[row, 6].Value = item.TotalAmount;
                    row++;
                    count++;
                }

                worksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;
                worksheet.Cells.AutoFitColumns();
                package.Save();
            }

            stream.Position = 0;
            string excelName = $"SaleOutReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
