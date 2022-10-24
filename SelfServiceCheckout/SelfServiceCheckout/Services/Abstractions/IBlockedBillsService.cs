namespace SelfServiceCheckout.Services.Abstractions
{
    public interface IBlockedBillsService
    {
        Task<int[]> GetBlockedBills();
    }
}