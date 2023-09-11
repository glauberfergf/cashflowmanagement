using AutoMapper;
using CashFlowManagement.Domain.DTO;
using CashFlowManagement.Domain.Entity;
using CashFlowManagement.Domain.Filters;
using CashFlowManagement.Domain.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CashFlowManagement.API.Controllers
{
    /// <summary>
    /// Controller responsible for launching payment orders    
    /// /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IMapper _mapper;
        private readonly IPaymentApplication _paymentApplication;

        /// <summary>
        /// Constructor of PaymentController
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="paymentApplication"></param>
        public PaymentController(ILogger<PaymentController> logger, IMapper mapper, IPaymentApplication paymentApplication)
        {
            _paymentApplication = paymentApplication;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria um novo lançamento.
        /// </summary>
        /// <param name="orderPayment">Os dados do lançamento a ser criado.</param>
        /// <response code="200">Retorna a confirmação do envio do lançamento, esse registro será processado posteriormente</response>
        /// <response code="400">Retorna informando que não foi encontrado uma order de pagamento na requisição</response>
        /// <response code="500">Retorna informando que ocorreu um erro</response>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] OrderPayment orderPayment)
        {
            _logger.LogInformation("PaymentController.Create - New Order Payment received (TransactionId: {orderPayment.TransactionId})", orderPayment.TransactionId);

            if (orderPayment is null)
                return BadRequest(orderPayment);

            var payment = _mapper.Map<Payment>(orderPayment);
            var result = await _paymentApplication.SendToQueue(payment);

            if (result)
            {
                _logger.LogInformation("PaymentController.Create - New Order Payment processed (TransactionId: {orderPayment.TransactionId})", orderPayment.TransactionId);
                return Ok(result);
            }
            _logger.LogWarning("PaymentController.Create - New Order Payment not processed (TransactionId: {orderPayment.TransactionId})", orderPayment.TransactionId);

            return StatusCode(StatusCodes.Status500InternalServerError, "Error to process your request, try again or contact the admin if return error again");
        }

        /// <summary>
        /// Retorna um lançamento.
        /// </summary>
        /// <param name="id">Id do registro do lançamento realizado</param>
        /// <response code="200">Retorna o registro de lançamento</response>
        /// <response code="400">Retorna informando que não foi encontrado o registro solicitado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Payment), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation("PaymentController.GetById (Id: {id})", id);

            if (id == Guid.Empty)
                return BadRequest(id);

            var result = await _paymentApplication.GetById(id);

            if (result is not null)
            {
                var paymentReport = _mapper.Map<PaymentReport>(result);
                _logger.LogInformation("PaymentController.GetById found (Id: {id})", id);
                return Ok(paymentReport);
            }
            _logger.LogInformation("PaymentController.GetById not found (Id: {id})", id);

            return NotFound();
        }

        /// <summary>
        /// GetByFilter
        /// </summary>
        /// <param name="filter">Parametros para realizar busca</param>
        /// <response code="200">Retorna o Relatório em formato de lista com todos os lançamentos encontrados a partir do filtro enviada</response>
        /// <response code="400">Retorna informando uma lista vazia</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Payment>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByFilter([FromQuery] PaymentFilter filter)
        {
            _logger.LogInformation($"PaymentController.GetByFilter");

            var result = await _paymentApplication.GetByFilter(filter);

            if (result is not null)
            {
                var paymentReport = _mapper.Map<IEnumerable<PaymentReport>>(result);
                _logger.LogInformation($"PaymentController.GetByFilter found");
                return Ok(paymentReport);
            }
            _logger.LogInformation($"PaymentController.GetByFilter not found");

            return NotFound();
        }


        /// <summary>
        /// Método Alternativo para realizar Pagamento.
        /// </summary>
        /// <param name="orderPayment">Os dados do lançamento a ser criado.</param>
        /// <response code="200">Retorna o resultado se foi realizado o pagamento com sucesso ou não</response>
        /// <response code="400">Retorna informando que não foi encontrado uma order de pagamento na requisição</response>
        /// <response code="500">Retorna informando que ocorreu um erro</response>
        [HttpPost]
        [Route("pay")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Pay([FromBody] OrderPayment orderPayment)
        {
            _logger.LogInformation("PaymentController.Pay - New Order Payment received (TransactionId: {orderPayment.TransactionId})", orderPayment.TransactionId);

            if (orderPayment is null)
                return BadRequest(orderPayment);

            var payment = _mapper.Map<Payment>(orderPayment);
            var result = await _paymentApplication.Pay(payment);

            if (result)
            {
                _logger.LogInformation("PaymentController.Pay - New Order Payment processed (TransactionId: {orderPayment.TransactionId})", orderPayment.TransactionId);
                return Ok(result);
            }

            _logger.LogWarning("PaymentController.Pay - New Order Payment not processed (TransactionId: {orderPayment.TransactionId})", orderPayment.TransactionId);

            return StatusCode(StatusCodes.Status500InternalServerError, "Error to process your request, try again or contact the admin if return error again");
        }


    }
}