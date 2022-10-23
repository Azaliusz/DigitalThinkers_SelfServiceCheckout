namespace SelfServiceCheckout.Exceptions
{
    public class NotPayableReturnException : SelfServiceCheckoutBaseException
    {
        public NotPayableReturnException(int returnValue)
            : base($"The machine cannot return {returnValue} based on the denominations stored in it.")
        {
        }
    }
}