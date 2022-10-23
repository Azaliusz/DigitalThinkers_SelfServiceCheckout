namespace SelfServiceCheckout.Exceptions
{
    public class NotEnoughCoverageException : SelfServiceCheckoutBaseException
    {
        public NotEnoughCoverageException(int price, double totalIncome) :
            base($"The total value of coins given ({totalIncome}) does not cover the price ({price}).")
        {
        }
    }
}