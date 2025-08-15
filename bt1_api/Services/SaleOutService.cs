using bt1_api.DBContext;
using bt1_api.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using web_api.Model;

namespace web_api.Services
{
    public class SaleOutService
    {
        private readonly ApplicationDbContext _context;

        public SaleOutService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SaleOutDTO>> GetSaleOutsAsync()
        {
            return await (from s in _context.SaleOut
                          join p in _context.MasterProduct on s.ProductId equals p.Id
                          select new SaleOutDTO
                          {
                              Id = s.Id,
                              ProductId = p.Id,
                              CustomerPoNo = s.CustomerPoNo,
                              OrderDate = s.OrderDate,
                              CustomerName = s.CustomerName,
                              ProductCode = p.ProductCode,
                              ProductName = p.ProductName,
                              Unit = p.Unit,
                              Price = s.Price,
                              Amount = s.Amount,
                              Quantity = s.Quantity,
                              BoxQuantity = s.BoxQuantity,
                              QuantityPerBox = s.QuantityPerBox
                          }).ToListAsync();
        }

        public async Task<SaleOut?> GetSaleOutByIdAsync(Guid id)
        {
            return await _context.SaleOut.FindAsync(id);
        }

        public async Task<bool> UpdateSaleOutAsync(Guid id, SaleOut saleOut)
        {
            if (id != saleOut.Id)
                return false;

            _context.Entry(saleOut).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SaleOut> CreateSaleOutAsync(SaleOut saleOut)
        {
            _context.SaleOut.Add(saleOut);
            await _context.SaveChangesAsync();
            return saleOut;
        }

        public async Task<bool> DeleteSaleOutAsync(Guid id)
        {
            var saleOut = await _context.SaleOut.FindAsync(id);
            if (saleOut == null)
                return false;

            _context.SaleOut.Remove(saleOut);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> UploadProductsAsync(List<SaleOut> saleOuts)
        {
            await _context.SaleOut.AddRangeAsync(saleOuts);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<SaleOutReport>> GetSaleOutReportAsync(int startDate, int endDate)
        {
            var paramStartDate = new SqlParameter("@StartDate", startDate);
            var paramEndDate = new SqlParameter("@EndDate", endDate);
            var sqlQuery = "SELECT * FROM dbo.fnSaleOutReport(@StartDate, @EndDate)";

            return await _context.SaleOutReports
                                 .FromSqlRaw(sqlQuery, paramStartDate, paramEndDate)
                                 .ToListAsync();
        }

        private bool SaleOutExists(Guid id)
        {
            return _context.SaleOut.Any(e => e.Id == id);
        }
    }
}
