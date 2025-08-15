namespace web_api.Model
{
    public class SaleOutReport
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
