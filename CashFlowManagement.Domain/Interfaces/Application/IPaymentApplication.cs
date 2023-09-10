using CashFlowManagement.Domain.DTO;
using CashFlowManagement.Domain.Entity;
using CashFlowManagement.Domain.Filters;

namespace CashFlowManagement.Domain.Interfaces.Application
{
    public interface IPaymentApplication
    {
        Task<bool> Pay(Payment payment);

        Task<bool> SendToQueue(Payment payment);
        Task CreateAsync(Payment payment);

        Task<Payment> GetById(Guid id);
        Task<IEnumerable<Payment>> GetByFilter(PaymentFilter filter);
        Task Update(Payment payment);
        Task Delete(Guid id);
    }
}