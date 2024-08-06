namespace FastFood.Models
{
    public class PaymentViewModel
    {
        public Payment Payment { get; set; }
        public List<CartItem> CartItems { get; set; }
        public decimal Total { get; set; }
    }
}
