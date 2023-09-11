using AutoMapper;
using CashFlowManagement.Domain.DTO;
using CashFlowManagement.Domain.Entity;

namespace CashFlowManagement.API.Helpers
{
    /// <summary>
    /// AutoMapperProfile class responsible for mapping classes
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// AutoMapperProfile config
        /// </summary>
        public AutoMapperProfile()
        {
            CreateMap<OrderPayment, Payment>();

            CreateMap<Payment, PaymentReport>();
        }
    }
}