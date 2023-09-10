namespace CashFlowManagement.AntiCorruption.Gateways
{
    public class EloGateway : IEloGateway
    {
        public async Task<Guid?> CommitTransaction()
        {
            var random = new Random().Next(2);

            if (random.Equals(1))
                return await Task.FromResult(Guid.NewGuid());

            return null;
        }
    }
}