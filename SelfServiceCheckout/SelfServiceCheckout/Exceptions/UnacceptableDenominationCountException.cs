namespace SelfServiceCheckout.Exceptions
{
    public class UnacceptableDenominationCountException : SelfServiceCheckoutBaseException
    {
        public UnacceptableDenominationCountException(int denomination, int count)
            : base($"The amount specified for denomination {denomination} is invalid:{count}")
        {
        }
    }
}