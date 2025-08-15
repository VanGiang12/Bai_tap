using bt1_api.DBContext;
using bt1_api.Model;
using Microsoft.EntityFrameworkCore;

namespace web_api.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<MasterProduct>> GetProductsAsync()
        {
            return await _context.MasterProduct.ToListAsync();
        }
        
        public async Task<MasterProduct?> GetProductByIdAsync(Guid id)
        {
            return await _context.MasterProduct.FindAsync(id);
        }
        
        public async Task<bool> UpdateProductAsync(Guid id, MasterProduct product)
        {
            if (id != product.Id) return false;

            _context.Entry(product).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.MasterProduct.AnyAsync(e => e.Id == id))
                    return false;
                throw;
            }
        }
        
        public async Task AddProductAsync(MasterProduct product)
        {
            _context.MasterProduct.Add(product);
            await _context.SaveChangesAsync();
        }
        
        public async Task AddProductsAsync(List<MasterProduct> products)
        {
            await _context.MasterProduct.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _context.MasterProduct.FindAsync(id);
            if (product == null) return false;

            _context.MasterProduct.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

