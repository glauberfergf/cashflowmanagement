using CashFlowManagement.Domain.Entity;

namespace CashFlowManagement.Domain.Services
{
    public interface ICreditCardFacade
    {
        Task<Guid?> Checkout(Payment payment);
    }
}
