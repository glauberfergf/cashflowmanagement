using CashFlowManagement.Domain.Entity;
using CashFlowManagement.Domain.Filters;

namespace CashFlowManagement.Domain.Interfaces.Infrastructure.Repository
{
    public interface IPaymentRepository
    {
        Task CreateAsync(Payment payment);
        Task UpdateAsync(Payment model);
        Task DeleteAsync(Guid id);
        Task<Payment> GetByIdAsync(Guid id);

        Task<IEnumerable<Payment>> GetByFilterAsync(PaymentFilter filter);
    }
}
