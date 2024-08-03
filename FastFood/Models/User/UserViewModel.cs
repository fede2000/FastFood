namespace FastFood.Models.User
{
    public class UserViewModel
    {
        public List<User> Users { get; set; } = new List<User>(); // Inicializa la lista para evitar null reference
        public User NewUser { get; set; } = new User();
    }
}
