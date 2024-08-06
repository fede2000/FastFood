namespace FastFood.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;

        public string Quantity { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public IFormFile ImageFile { get; set; } // Propiedad para el archivo de imagen}

        public string? CategoryName { get; set; } // Añade esta propiedad
    }
}
