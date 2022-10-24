namespace SelfServiceCheckout.Exceptions
{
    public class NegativePriceValueException : SelfServiceCheckoutBaseException
    {
        public NegativePriceValueException(int price)
            : base($"The checkout price can not be negative value: {price}.")
        {
        }
    }
}