namespace FastFood.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string Name { get; set; }
        public string CardNo { get; set; }
        public int ExpireMonth { get; set; } // Agrega esta propiedad
        public int ExpireYear { get; set; }  // Agrega esta propiedad
        public DateTime ExpireDate { get; set; }
        public int? Cvv { get; set; }
        public string Address { get; set; }
        public string PaymentMode { get; set; }

        public decimal Amount { get; set; }
    }
}
