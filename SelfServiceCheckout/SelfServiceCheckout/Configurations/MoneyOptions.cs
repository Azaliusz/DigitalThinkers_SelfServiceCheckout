using SelfServiceCheckout.Models;

namespace SelfServiceCheckout.Configurations
{
    public class MoneyOptions
    {
        public Currencies DefaultCurrency { get; set; }

        public Dictionary<Currencies, int[]>? AcceptableDenominations { get; set; }
    }
}