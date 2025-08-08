using bt1_api.Model;
using Newtonsoft.Json;

namespace web_api.Model
{
    public class SaleOut
    {
        public Guid Id { get; set; }
        public string CustomerPoNo { get; set; }
        public int OrderDate { get; set; }
        public string CustomerName { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityPerBox { get; set; }
        public decimal BoxQuantity { get; set; }

        public Guid ProductId { get; set; }


    }
}
