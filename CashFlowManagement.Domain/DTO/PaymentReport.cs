using CashFlowManagement.Domain.Enums;

namespace CashFlowManagement.Domain.DTO
{
    public class PaymentReport
    {
        public PaymentOrigin PaymentOrigin { get; set; }
        public Status Status { get; set; }
        public PaymentType PaymentType { get; set; }
        public string PaymentDescription { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
