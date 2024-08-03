namespace FastFood.Models.Category
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public IFormFile ImageFile { get; set; } // Propiedad para el archivo de imagen
    }
}
