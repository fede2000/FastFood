using FastFood.Models;

namespace FastFood.Models
{
    public class OrderViewModel
    {
        public OrderDetailsViewModel UpdateOrder { get; set; }
        public List<OrderDetailsViewModel> Orders { get; set; } = new List<OrderDetailsViewModel>();
    }
}
