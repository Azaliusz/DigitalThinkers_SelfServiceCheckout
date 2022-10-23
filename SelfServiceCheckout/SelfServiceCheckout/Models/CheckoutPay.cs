using System.ComponentModel.DataAnnotations;

namespace SelfServiceCheckout.Models
{
    public class CheckoutPay
    {
        [Required]
        public Dictionary<int, int>? Inserted { get; set; }

        [Required]
        public int Price { get; set; }

        public Currencies? Currency { get; set; }
    }
}