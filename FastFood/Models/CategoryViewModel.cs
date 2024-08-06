namespace FastFood.Models
{
    public class CategoryViewModel
    {
        public List<Category> Categories { get; set; } = new List<Category>(); // Inicializa la lista para evitar null reference
        public Category NewCategory { get; set; } = new Category();
    }


}
