namespace FastFood.Models
{
    public class ProductViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>(); // Inicializa la lista para evitar null reference
        public Product NewProduct { get; set; } = new Product();

        public List<Category> Categories { get; set; } = new List<Category>();
    }


}
