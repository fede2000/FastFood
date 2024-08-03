namespace FastFood.Models.Order
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public int PaymentId { get; set; }
        public string PaymentMode { get; set; }

        // Inicializa la lista de productos para evitar null references
        public List<ProductDetailsViewModel> Products { get; set; } = new List<ProductDetailsViewModel>();
    }

    public class ProductDetailsViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
