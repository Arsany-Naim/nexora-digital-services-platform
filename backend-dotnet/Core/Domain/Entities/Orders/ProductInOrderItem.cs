namespace Domain.Entities.Orders
{
    public class ProductInOrderItem
    {
        public ProductInOrderItem(int productId, string productName, string productPicture)
        {
            ProductId = productId;
            ProductName = productName;
            PictureUrl = productPicture;
        }
        public ProductInOrderItem()
        {
            
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
    }
}