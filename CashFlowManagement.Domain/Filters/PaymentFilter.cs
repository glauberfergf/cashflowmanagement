using CashFlowManagement.Domain.Enums;

namespace CashFlowManagement.Domain.Filters
{
    public class PaymentFilter
    {
        public DateTime? DateOnly { get; set; }

        public DateTime CreatedAtMin { get; set; }
        public DateTime CreatedAtMax { get; set; }

        public Status? Status { get; set; }
    }
}
