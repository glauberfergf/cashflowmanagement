using CashFlowManagement.Domain.Entity;

namespace CashFlowManagement.Domain.Services
{
    public interface IDebitCardFacade
    {
        Task<Guid?> Checkout(Payment payment);
    }
}
