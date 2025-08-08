using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
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
            // Đảm bảo model binding thành công
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

    }
}
