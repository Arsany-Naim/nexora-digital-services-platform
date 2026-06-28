namespace Domain.Entities.Orders
{
    public class OrderItem : BaseEntity<int>
    {
        public OrderItem(ProductInOrderItem product, decimal price, int quantity)
        {
            Product = product;
            Price = price;
            Quantity = quantity;
        }
        public OrderItem()
        {
            
        }

        public ProductInOrderItem Product { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}