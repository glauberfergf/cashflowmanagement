using CashFlowManagement.Application.Implementation;
using CashFlowManagement.Domain.Entity;
using CashFlowManagement.Domain.Enums;
using CashFlowManagement.Domain.Filters;
using CashFlowManagement.Domain.Interfaces.Infrastructure.RabbitMq;
using CashFlowManagement.Domain.Interfaces.Infrastructure.Repository;
using CashFlowManagement.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CashFlowManagement.Test.Application
{
    [TestFixture]
    public class PaymentApplicationTests
    {
        [Test]
        public async Task SendToQueue_Success()
        {
            // Arrange
            var payment = new Payment(); // Crie um objeto Payment válido para o teste
            var mockRabbitMqClient = new Mock<IRabbitMqClient>();
            mockRabbitMqClient.Setup(client => client.SendToQueue(payment)).ReturnsAsync(true);
            var logger = new Mock<ILogger<PaymentApplication>>();
            var repository = new Mock<IPaymentRepository>();
            var creditCardFacade = new Mock<ICreditCardFacade>();
            var debitCardFacade = new Mock<IDebitCardFacade>();

            var paymentApplication = new PaymentApplication(mockRabbitMqClient.Object, repository.Object, creditCardFacade.Object, debitCardFacade.Object, logger.Object);

            // Act
            var result = await paymentApplication.SendToQueue(payment);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task SendToQueue_Failure()
        {
            // Arrange
            var payment = new Payment(); // Crie um objeto Payment válido para o teste
            var mockRabbitMqClient = new Mock<IRabbitMqClient>();
            mockRabbitMqClient.Setup(client => client.SendToQueue(payment)).ReturnsAsync(false);
            var logger = new Mock<ILogger<PaymentApplication>>();
            var repository = new Mock<IPaymentRepository>();
            var creditCardFacade = new Mock<ICreditCardFacade>();
            var debitCardFacade = new Mock<IDebitCardFacade>();
            var paymentApplication = new PaymentApplication(mockRabbitMqClient.Object, repository.Object, creditCardFacade.Object, debitCardFacade.Object, logger.Object);

            // Act
            var result = await paymentApplication.SendToQueue(payment);

            // Act & Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CreateAsync_StorePayment_ShouldSetStatusApproved()
        {
            // Arrange
            var payment = new Payment { PaymentOrigin = PaymentOrigin.Store };
            var mockPaymentRepository = new Mock<IPaymentRepository>();
            var mockRabbitMqClient = new Mock<IRabbitMqClient>();
            var logger = new Mock<ILogger<PaymentApplication>>();
            var creditCardFacade = new Mock<ICreditCardFacade>();
            var debitCardFacade = new Mock<IDebitCardFacade>();
            var paymentApplication = new PaymentApplication(mockRabbitMqClient.Object, mockPaymentRepository.Object, creditCardFacade.Object, debitCardFacade.Object, logger.Object);

            // Act
            await paymentApplication.CreateAsync(payment);

            // Assert
            Assert.That(payment.Status, Is.EqualTo(Status.Approved));
            mockPaymentRepository.Verify(repo => repo.CreateAsync(payment), Times.Once);
        }

        [Test]
        public async Task Update_ShouldCallPaymentRepositoryUpdateAsync()
        {
            // Arrange
            var payment = new Payment();
            var mockPaymentRepository = new Mock<IPaymentRepository>();
            var mockRabbitMqClient = new Mock<IRabbitMqClient>();
            var logger = new Mock<ILogger<PaymentApplication>>();
            var creditCardFacade = new Mock<ICreditCardFacade>();
            var debitCardFacade = new Mock<IDebitCardFacade>();
            var paymentApplication = new PaymentApplication(mockRabbitMqClient.Object, mockPaymentRepository.Object, creditCardFacade.Object, debitCardFacade.Object, logger.Object);

            // Act
            await paymentApplication.Update(payment);

            // Assert
            mockPaymentRepository.Verify(repo => repo.UpdateAsync(payment), Times.Once);
        }

        [Test]
        public async Task Delete_ShouldCallPaymentRepositoryDeleteAsync()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var mockPaymentRepository = new Mock<IPaymentRepository>();
            var mockRabbitMqClient = new Mock<IRabbitMqClient>();
            var logger = new Mock<ILogger<PaymentApplication>>();
            var creditCardFacade = new Mock<ICreditCardFacade>();
            var debitCardFacade = new Mock<IDebitCardFacade>();
            var paymentApplication = new PaymentApplication(mockRabbitMqClient.Object, mockPaymentRepository.Object, creditCardFacade.Object, debitCardFacade.Object, logger.Object);

            // Act
            await paymentApplication.Delete(paymentId);

            // Assert
            mockPaymentRepository.Verify(repo => repo.DeleteAsync(paymentId), Times.Once);
        }

        [Test]
        public async Task GetById_ShouldReturnCorrectPayment()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var expectedPayment = new Payment { Id = paymentId };
            var mockPaymentRepository = new Mock<IPaymentRepository>();
            mockPaymentRepository.Setup(repo => repo.GetByIdAsync(paymentId)).ReturnsAsync(expectedPayment);
            var mockRabbitMqClient = new Mock<IRabbitMqClient>();
            var logger = new Mock<ILogger<PaymentApplication>>();
            var creditCardFacade = new Mock<ICreditCardFacade>();
            var debitCardFacade = new Mock<IDebitCardFacade>();
            var paymentApplication = new PaymentApplication(mockRabbitMqClient.Object, mockPaymentRepository.Object, creditCardFacade.Object, debitCardFacade.Object, logger.Object);

            // Act
            var result = await paymentApplication.GetById(paymentId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedPayment));
        }

        [Test]
        public async Task GetByFilter_ShouldReturnFilteredPayments()
        {
            // Arrange
            var filter = new PaymentFilter { /* Defina seus filtros aqui */ };
            var expectedPayments = new List<Payment> { /* Crie pagamentos que correspondam aos filtros aqui */ };
            var mockPaymentRepository = new Mock<IPaymentRepository>();
            mockPaymentRepository.Setup(repo => repo.GetByFilterAsync(filter)).ReturnsAsync(expectedPayments);
            var mockRabbitMqClient = new Mock<IRabbitMqClient>();
            var logger = new Mock<ILogger<PaymentApplication>>();
            var creditCardFacade = new Mock<ICreditCardFacade>();
            var debitCardFacade = new Mock<IDebitCardFacade>();
            var paymentApplication = new PaymentApplication(mockRabbitMqClient.Object, mockPaymentRepository.Object, creditCardFacade.Object, debitCardFacade.Object, logger.Object);

            // Act
            var result = await paymentApplication.GetByFilter(filter);

            // Assert
            CollectionAssert.AreEqual(expectedPayments, result);
        }

    }
}