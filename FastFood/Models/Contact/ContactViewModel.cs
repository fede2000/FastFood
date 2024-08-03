namespace FastFood.Models.Contact
{
    public class ContactViewModel
    {
        public List<Contact> Contacts { get; set; } = new List<Contact>(); // Inicializa la lista para evitar null reference
        public Contact NewContact { get; set; } = new Contact();
    }
}
