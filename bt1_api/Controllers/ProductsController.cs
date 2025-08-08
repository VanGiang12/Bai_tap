using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bt1_api.DBContext;
using bt1_api.Model;

namespace web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MasterProduct>>> GetProducts()
        {
            return await _context.MasterProduct.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MasterProduct>> GetProduct(Guid id)
        {
            var product = await _context.MasterProduct.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct( Guid id, MasterProduct product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<MasterProduct>> PostProduct([FromBody]MasterProduct product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.MasterProduct.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Add successfully!" });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadProducts([FromBody] List<MasterProduct> products)
        {
            if (products == null || !products.Any())
            {
                return BadRequest("Danh sách sản phẩm rỗng.");
            }

            await _context.MasterProduct.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Upload thành công", count = products.Count });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {

            var product = await _context.MasterProduct.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.MasterProduct.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(Guid id)
        {
            return _context.MasterProduct.Any(e => e.Id == id);
        }
    }
}
