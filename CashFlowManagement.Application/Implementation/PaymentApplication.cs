using CashFlowManagement.Domain.Entity;
using CashFlowManagement.Domain.Enums;
using CashFlowManagement.Domain.Filters;
using CashFlowManagement.Domain.Interfaces.Application;
using CashFlowManagement.Domain.Interfaces.Infrastructure.RabbitMq;
using CashFlowManagement.Domain.Interfaces.Infrastructure.Repository;
using CashFlowManagement.Domain.Services;
using Microsoft.Extensions.Logging;

namespace CashFlowManagement.Application.Implementation
{
    public class PaymentApplication : IPaymentApplication
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentApplication> _logger;
        private readonly IRabbitMqClient _rabbitMqClient;

        private readonly ICreditCardFacade _creditCardFacade;
        private readonly IDebitCardFacade _debitCardFacade;

        public PaymentApplication(IRabbitMqClient rabbitMqClient, IPaymentRepository paymentRepository, ICreditCardFacade creditCardFacade, IDebitCardFacade debitCardFacade, ILogger<PaymentApplication> logger)
        {
            _paymentRepository = paymentRepository;
            _rabbitMqClient = rabbitMqClient;
            _creditCardFacade = creditCardFacade;
            _debitCardFacade = debitCardFacade;
            _logger = logger;
        }

        public async Task<bool> SendToQueue(Payment payment)
        {
            bool ret = false;
            try
            {
                ret = await _rabbitMqClient.SendToQueue(payment);
            }
            catch (Exception e)
            {
                _logger.LogError("Payment Application: Error to publish message to Queue (TransactionId: {payment.TransactionId}, Message: {e.Message})", payment.TransactionId, e.Message);
            }

            return await Task.FromResult(ret);
        }

        public async Task CreateAsync(Payment payment)
        {
            switch (payment.PaymentOrigin)
            {
                case PaymentOrigin.Internet:
                    break;
                case PaymentOrigin.Mobile:
                    break;
                case PaymentOrigin.Store:
                    payment.Status = Status.Approved;
                    break;
                default:
                    _logger.LogError("PaymentApplication.CreateAsync: PaymentOrigin with option not found (option: {payment.PaymentOrigin}) (TransactionId: {payment.TransactionId})", payment.PaymentOrigin, payment.TransactionId);
                    break;
            }

            await _paymentRepository.CreateAsync(payment);
        }

        public async Task Update(Payment payment)
        {
            await _paymentRepository.UpdateAsync(payment);
        }

        public async Task Delete(Guid id)
        {
            await _paymentRepository.DeleteAsync(id);
        }

        public async Task<Payment> GetById(Guid id)
        {
            return await _paymentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Payment>> GetByFilter(PaymentFilter filter)
        {
            return await _paymentRepository.GetByFilterAsync(filter);
        }

        public async Task<bool> Pay(Payment payment)
        {
            bool ret = false;

            Guid? transaction = null;
            switch (payment.PaymentType)
            {
                case PaymentType.Credit:
                    transaction = await _creditCardFacade.Checkout(payment);
                    break;
                case PaymentType.Debit:
                    transaction = await _debitCardFacade.Checkout(payment);
                    break;
                default:
                    break;
            }

            if (transaction is null)
            {
                payment.Status = Status.Refused;
                ret = false;
            }
            else
            {
                payment.TransactionId = transaction.Value;
                payment.Status = Status.Approved;
                ret = true;
            }

            await _paymentRepository.CreateAsync(payment);

            return await Task.FromResult<bool>(ret);
        }

    }
}