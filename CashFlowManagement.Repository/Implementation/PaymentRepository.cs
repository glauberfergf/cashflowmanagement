using CashFlowManagement.Domain.Entity;
using CashFlowManagement.Domain.Filters;
using CashFlowManagement.Domain.Interfaces.Infrastructure.Repository;
using Dapper;
using CashFlowManagement.Domain.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Extensions.Options;
using CashFlowManagement.Domain.Enums;

namespace CashFlowManagement.Repository.Implementation
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DbSettings _dbSettings;

        public PaymentRepository(IOptions<DbSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new MySqlConnection(_dbSettings.ConnectionString);
        }

        public async Task CreateAsync(Payment payment)
        {
            using var connection = CreateConnection();
            connection.Open();
            var sql = @"INSERT INTO Payment (Id, TransactionId, PaymentOrigin, Status, PaymentType, PaymentDescription, Amount, PaymentDate, CreatedDate)
                            VALUES (uuid(), @TransactionId, @PaymentOrigin, @Status, @PaymentType, @PaymentDescription, @Amount, @PaymentDate, now())";
            await connection.ExecuteAsync(sql, payment);
        }

        public async Task<IEnumerable<Payment>> GetByFilterAsync(PaymentFilter filter)
        {
            var dictionary = new Dictionary<string, object>();
            var sql = @"SELECT * FROM Payment WHERE ";

            if (filter.DateOnly.HasValue)
            {
                dictionary.Add("@DateOnly", filter.DateOnly.Value.Date);
                sql += @"  DATE(CreatedDate) = @DateOnly";
            }
            else
            {
                dictionary.Add("@CreatedAtMin", filter.CreatedAtMin);
                dictionary.Add("@CreatedAtMax", filter.CreatedAtMax);
                sql += @"  CreatedDate >= @CreatedAtMin and CreatedDate <= @CreatedAtMax";
            }

            if (filter.Status.HasValue)
            {
                dictionary.Add("@Status", filter.Status);
                sql += @" and Status = @Status";
            }

            using var connection = CreateConnection();            
            return await connection.QueryAsync<Payment>(sql, new DynamicParameters(dictionary));
        }

        public async Task UpdateAsync(Payment payment)
        {
            using var connection = CreateConnection();
            connection.Open();
            var sql = @"UPDATE Payment SET
                                Status = @Status,
                                PaymentDescription = @PaymentDescription
                                WHERE Id = @Id";
            await connection.ExecuteAsync(sql, payment);
        }

        public async Task DeleteAsync(Guid id)
        {
            using var connection = CreateConnection();
            connection.Open();
            var sql = @"DELETE FROM Payment WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<Payment> GetByIdAsync(Guid id)
        {
            using var connection = CreateConnection();
            connection.Open();
            var sql = @"select * from Payment WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Payment>(sql, new { Id = id });
        }
    }
}
