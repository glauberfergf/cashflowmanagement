using CashFlowManagement.Domain.Enums;

namespace CashFlowManagement.Domain.Entity
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public PaymentOrigin PaymentOrigin { get; set; }
        public Status Status { get; set; } = Status.Processing;
        public PaymentType PaymentType { get; set; }
        public string PaymentDescription { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}