using Microsoft.AspNetCore.Mvc;
using bt1_api.Model;
using web_api.Services;

namespace web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MasterProduct>>> GetProducts()
        {
            return await _productService.GetProductsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MasterProduct>> GetProduct(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return product;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(Guid id, MasterProduct product)
        {
            var result = await _productService.UpdateProductAsync(id, product);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct(MasterProduct product)
        {
            await _productService.AddProductAsync(product);
            return Ok(new { message = "Add successfully!" });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadProducts(List<MasterProduct> products)
        {
            if (products == null || !products.Any())
                return BadRequest("Danh sách sản phẩm rỗng.");

            await _productService.AddProductsAsync(products);
            return Ok(new { message = "Upload thành công", count = products.Count });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
