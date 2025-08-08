using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bt1_api.DBContext;
using web_api.Model;

namespace web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOutsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SaleOutsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SaleOuts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleOutDTO>>> GetSaleOuts()
        {
            return await (from s in _context.SaleOut
                          join p in _context.MasterProduct on s.ProductId equals p.Id
                          select new SaleOutDTO
                          {
                              Id= s.Id,
                              ProductId= p.Id,
                              CustomerPoNo = s.CustomerPoNo,
                              OrderDate = s.OrderDate,
                              CustomerName = s.CustomerName,
                              ProductCode=p.ProductCode,
                              ProductName = p.ProductName,
                              Unit = p.Unit,
                              Quantity = s.Quantity,
                              BoxQuantity = s.BoxQuantity,
                              QuantityPerBox = p.QuantityPerBox
                          }).ToListAsync();
        }

        // GET: api/SaleOuts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleOut>> GetSaleOut(Guid id)
        {
            var saleOut = await _context.SaleOut.FindAsync(id);

            if (saleOut == null)
            {
                return NotFound();
            }

            return saleOut;
        }

        // PUT: api/SaleOuts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSaleOut(Guid id, SaleOut saleOut)
        {
            if (id != saleOut.Id)
            {
                return BadRequest();
            }

            _context.Entry(saleOut).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleOutExists(id))
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

        // POST: api/SaleOuts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SaleOut>> PostSaleOut([FromBody] SaleOut saleOut)
        {
            _context.SaleOut.Add(saleOut);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSaleOut", new { id = saleOut.Id }, saleOut);
        }

        // DELETE: api/SaleOuts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleOut(Guid id)
        {
            var saleOut = await _context.SaleOut.FindAsync(id);
            if (saleOut == null)
            {
                return NotFound();
            }

            _context.SaleOut.Remove(saleOut);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SaleOutExists(Guid id)
        {
            return _context.SaleOut.Any(e => e.Id == id);
        }
    }
}
