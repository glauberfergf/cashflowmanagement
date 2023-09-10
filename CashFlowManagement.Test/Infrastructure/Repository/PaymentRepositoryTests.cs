using Moq;
using CashFlowManagement.Domain.Configuration;
using CashFlowManagement.Domain.Entity;
using CashFlowManagement.Repository.Implementation;
using Microsoft.Extensions.Options;
using Dapper;
using CashFlowManagement.Domain.Enums;
using CashFlowManagement.Domain.Filters;
using MySql.Data.MySqlClient;

namespace CashFlowManagement.Test.Infrastructure.Repository
{
    [TestFixture]
    public class PaymentRepositoryTests
    {
        private PaymentRepository _paymentRepository;
        private Mock<IOptions<DbSettings>> _mockDbSettings;

        [SetUp]
        public void Setup()
        {
            // Configuração padrão para testes
            _mockDbSettings = new Mock<IOptions<DbSettings>>();
            _mockDbSettings.Setup(dbSettings => dbSettings.Value).Returns(new DbSettings
            {
                ConnectionString = "Server=localhost; Database=CashFlowManagement; Uid=root; Pwd=senha_forte!"
            });

            _paymentRepository = new PaymentRepository(_mockDbSettings.Object);
        }

        [Test]
        public async Task CreateAsync_Should_Insert_Payment()
        {
            // Arrange
            var payment = new Payment
            {
                TransactionId = Guid.NewGuid(),
                PaymentOrigin = PaymentOrigin.Internet,
                Status = Status.Approved,
                PaymentType = PaymentType.Credit,
                PaymentDescription = "Test Payment",
                Amount = 100,
                PaymentDate = DateTime.UtcNow
            };

            // Act
            await _paymentRepository.CreateAsync(payment);

            // Assert
            var result = await GetByTransactionId(payment.TransactionId);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.PaymentOrigin, Is.EqualTo(payment.PaymentOrigin));
                Assert.That(result.Status, Is.EqualTo(payment.Status));
                Assert.That(result.PaymentType, Is.EqualTo(payment.PaymentType));
                Assert.That(result.PaymentDescription, Is.EqualTo(payment.PaymentDescription));
                Assert.That(result.Amount, Is.EqualTo(payment.Amount));
            });
        }

        [Test]
        public async Task GetByFilterAsync_Should_Return_Filtered_Payments()
        {
            // Arrange
            var filter = new PaymentFilter
            {
                CreatedAtMin = DateTime.UtcNow.AddHours(-48),
                CreatedAtMax = DateTime.UtcNow,
            };

            // Act
            var result = await _paymentRepository.GetByFilterAsync(filter);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            foreach (var payment in result)
            {
                Assert.That(payment.CreatedDate >= filter.CreatedAtMin && payment.CreatedDate <= filter.CreatedAtMax, Is.True);
            }
        }

        [Test]
        public async Task UpdateAsync_Should_Update_Payment()
        {
            // Arrange
            var payment = new Payment
            {
                TransactionId = Guid.NewGuid(),
                PaymentOrigin = PaymentOrigin.Internet,
                Status = Status.Processing,
                PaymentType = PaymentType.Debit,
                PaymentDescription = "Test Payment",
                Amount = 50,
                PaymentDate = DateTime.UtcNow
            };

            await _paymentRepository.CreateAsync(payment);
            var paymentInserted = await GetByTransactionId(payment.TransactionId);

            payment.Id = paymentInserted.Id;
            payment.Status = Status.Approved;

            // Act
            await _paymentRepository.UpdateAsync(payment);

            // Assert
            var paymentUpdated = await GetByTransactionId(payment.TransactionId);
            Assert.Multiple(() =>
            {
                Assert.That(paymentUpdated, Is.Not.Null);
                Assert.That(paymentUpdated.Status, Is.EqualTo(Status.Approved));
            });
        }

        private async Task<Payment> GetByTransactionId(Guid transactionId)
        {
            using var connection = new MySqlConnection(_mockDbSettings.Object.Value.ConnectionString);
            connection.Open();
            var sql = "SELECT * FROM Payment WHERE TransactionId = @transactionId";
            return await connection.QuerySingleOrDefaultAsync<Payment>(sql, new { transactionId });
        }

        [Test]
        public async Task DeleteAsync_Should_Delete_Payment()
        {
            // Arrange
            var payment = new Payment
            {
                TransactionId = Guid.NewGuid(),
                PaymentOrigin = PaymentOrigin.Internet,
                Status = Status.Approved,
                PaymentType = PaymentType.Credit,
                PaymentDescription = "Test Payment",
                Amount = 100,
                PaymentDate = DateTime.UtcNow
            };

            await _paymentRepository.CreateAsync(payment);
            var paymentCreated = await GetByTransactionId(payment.TransactionId);

            // Act
            await _paymentRepository.DeleteAsync(paymentCreated.Id);

            // Assert
            var result = await GetByTransactionId(paymentCreated.TransactionId);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Payment()
        {
            // Arrange
            var payment = new Payment
            {
                TransactionId = Guid.NewGuid(),
                PaymentOrigin = PaymentOrigin.Internet,
                Status = Status.Approved,
                PaymentType = PaymentType.Credit,
                PaymentDescription = "Test Payment",
                Amount = 100,
                PaymentDate = DateTime.UtcNow
            };

            await _paymentRepository.CreateAsync(payment);
            var paymentCreated = await GetByTransactionId(payment.TransactionId);


            // Act
            var result = await _paymentRepository.GetByIdAsync(paymentCreated.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TransactionId, Is.EqualTo(payment.TransactionId));
        }
    }
}