using FastFood.Models.Order;

namespace FastFood.Models.Contact
{
    public class OrderViewModel
    {
        public OrderDetailsViewModel UpdateOrder { get; set; }
        public List<OrderDetailsViewModel> Orders { get; set; } = new List<OrderDetailsViewModel>();
    }
}
