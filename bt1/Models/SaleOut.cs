using System.Text.Json.Serialization;

namespace web_app.Models
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
        public SaleOut() { }
        public SaleOut(string customerPoNo, int orderDate, string customerName, decimal quantity, decimal quantityPerBox, decimal boxQuantity, Guid productId)
        {
            CustomerPoNo = customerPoNo;
            OrderDate = orderDate;
            CustomerName = customerName;
            Quantity = quantity;
            QuantityPerBox = quantityPerBox;
            BoxQuantity = boxQuantity;
            ProductId = productId;
        }
    }
}
