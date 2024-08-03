using FastFood.Models.Cart;

namespace FastFood.Models.Payment
{
    public class PaymentViewModel
    {
        public Payment Payment { get; set; }
        public List<CartItem> CartItems { get; set; }
        public decimal Total { get; set; }
    }
}
