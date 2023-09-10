using CashFlowManagement.AntiCorruption.Gateways;
using CashFlowManagement.Domain.Entity;
using CashFlowManagement.Domain.Services;

namespace CashFlowManagement.AntiCorruption.Facades
{
    public class CreditCardFacade : ICreditCardFacade
    {
        private readonly IPayPalGateway _payPalGateway;
        public CreditCardFacade(IPayPalGateway payPalGateway)
        {
            _payPalGateway = payPalGateway;
        }

        public async Task<Guid?> Checkout(Payment payment)
        {
            return await _payPalGateway.CommitTransaction();
        }
    }
}