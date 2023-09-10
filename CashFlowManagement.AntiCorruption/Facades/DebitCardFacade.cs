using CashFlowManagement.AntiCorruption.Gateways;
using CashFlowManagement.Domain.Entity;
using CashFlowManagement.Domain.Services;

namespace CashFlowManagement.AntiCorruption.Facades
{
    public class DebitCardFacade : IDebitCardFacade
    {
        private readonly IEloGateway _eloGateway;

        public DebitCardFacade(IEloGateway eloGateway)
        {
            _eloGateway = eloGateway;
        }

        public async Task<Guid?> Checkout(Payment payment)
        {
            return await _eloGateway.CommitTransaction();
        }
    }
}