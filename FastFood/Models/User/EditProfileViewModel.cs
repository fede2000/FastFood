namespace FastFood.Models.User
{
    public class EditProfileViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public IFormFile ImageFile { get; set; }
        public string ImageUrl { get; set; }
    }
}
