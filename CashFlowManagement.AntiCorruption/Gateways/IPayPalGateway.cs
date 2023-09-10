namespace CashFlowManagement.AntiCorruption.Gateways
{
    public interface IPayPalGateway
    {
        Task<Guid?> CommitTransaction();
    }
}