using CashFlowManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CashFlowManagement.Domain.DTO
{
    public class OrderPayment
    {
        public Guid TransactionId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public PaymentType PaymentType { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }
        [Required]
        public PaymentOrigin PaymentOrigin { get; set; }
        public string PaymentDescription { get; set; } = null!;
    }
}