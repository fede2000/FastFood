namespace FastFood.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string PostCode { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty; // Propiedad para el archivo de imagen
        public IFormFile ImageFile { get; set; } // Propiedad para el archivo de imagen}

        public DateTime CreatedDate { get; set; }

        public string Password { get; set; } // Propiedad para la contraseña hasheada

        public TipoUsuario TipoUsuario { get; set; }

    }

}
