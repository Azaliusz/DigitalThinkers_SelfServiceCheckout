namespace SelfServiceCheckout.Exceptions
{
    public class UnsupportedDenominationException : SelfServiceCheckoutBaseException
    {
        public UnsupportedDenominationException(int denomination)
            : base($"The given denomination {denomination} is not acceptable.")
        {
        }
    }
}