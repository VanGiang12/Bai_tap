using bt1_api.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using web_api.Model;
using web_api.Services;

namespace web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOutsController : ControllerBase
    {
        private readonly SaleOutService _saleOutService;

        public SaleOutsController(SaleOutService saleOutService)
        {
            _saleOutService = saleOutService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleOutDTO>>> GetSaleOuts()
        {
            return await _saleOutService.GetSaleOutsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleOut>> GetSaleOut(Guid id)
        {
            var saleOut = await _saleOutService.GetSaleOutByIdAsync(id);
            if (saleOut == null)
                return NotFound();

            return saleOut;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSaleOut(Guid id, SaleOut saleOut)
        {
            var result = await _saleOutService.UpdateSaleOutAsync(id, saleOut);
            if (!result)
                return BadRequest();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<SaleOut>> PostSaleOut([FromBody] SaleOut saleOut)
        {
            var created = await _saleOutService.CreateSaleOutAsync(saleOut);
            return CreatedAtAction(nameof(GetSaleOut), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleOut(Guid id)
        {
            var result = await _saleOutService.DeleteSaleOutAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadProducts([FromBody] List<SaleOut> saleOuts)
        {
            if (saleOuts == null || saleOuts.Count == 0)
                return BadRequest("Danh sách saleout rỗng.");

            var count = await _saleOutService.UploadProductsAsync(saleOuts);
            return Ok(new { message = "Upload thành công", count });
        }

        [HttpGet("saleout-report")]
        public async Task<IActionResult> GetSaleOutReport(int startDate, int endDate)
        {
            var reportData = await _saleOutService.GetSaleOutReportAsync(startDate, endDate);
            return Ok(reportData);
        }
    }
}
