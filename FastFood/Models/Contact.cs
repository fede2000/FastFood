using System.ComponentModel.DataAnnotations;

namespace FastFood.Models
{
    public class Contact
    {
        public int ContactId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El asunto es obligatorio")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "El mensaje es obligatorio")]
        public string Message { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
