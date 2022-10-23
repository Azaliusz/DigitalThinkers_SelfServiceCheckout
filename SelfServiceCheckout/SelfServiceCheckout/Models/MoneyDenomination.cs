namespace SelfServiceCheckout.Models
{
    public class MoneyDenomination
    {
        public Currencies Currency { get; set; }

        public int Denomination { get; set; }

        public int Count { get; set; }
    }
}