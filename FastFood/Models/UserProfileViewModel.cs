using FastFood.Models;

namespace FastFood.Models
{
    public class UserProfileViewModel
    {
        public IEnumerable<OrderDetailsViewModel> Orders { get; set; } = new List<OrderDetailsViewModel>(); // Inicializa la lista para evitar null reference
        public User User { get; set; } = new User();


    }
}
