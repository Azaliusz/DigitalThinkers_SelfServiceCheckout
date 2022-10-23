using SelfServiceCheckout.Models;

namespace SelfServiceCheckout.Configurations
{
    public class MoneyOptions
    {
        public Dictionary<Currencies, int[]>? AcceptableDenominations { get; set; }
    }
}