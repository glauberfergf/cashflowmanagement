namespace CashFlowManagement.AntiCorruption.Gateways
{
    public interface IEloGateway
    {
        Task<Guid?> CommitTransaction();
    }
}