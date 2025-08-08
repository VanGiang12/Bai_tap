namespace web_app.Models
{
    public class MasterProduct
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public string Specification { get; set; }
        public decimal QuantityPerBox { get; set; }
        public decimal ProductWeight { get; set; }

        public MasterProduct() { }
        public MasterProduct(string productCode, string productName, string unit, string specification, decimal quantityPerBox, decimal productWeight)
        {
            ProductCode = productCode;
            ProductName = productName;
            Unit = unit;
            Specification = specification;
            QuantityPerBox = quantityPerBox;
            ProductWeight = productWeight;
        }
    }
}
