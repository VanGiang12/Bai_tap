namespace web_api.Model
{
    public class SaleOutDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string CustomerPoNo { get; set; }
        public int OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityPerBox { get; set; }
        public decimal BoxQuantity { get; set; }
    }
}
