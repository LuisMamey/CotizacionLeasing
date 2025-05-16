using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.Results;
using CotizacionLeasing.Application.DTOs;
using CotizacionLeasing.Application.Interfaces;

namespace CotizacionLeasing.API.Controllers
{
    /// <summary>
    /// Controlador REST para operaciones de cotizaciones de arrendamiento:
    /// permite calcular, guardar y recuperar cotizaciones por cliente.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        /// <summary>
        /// Servicio de negocio que orquesta la creación y recuperación de cotizaciones.
        /// </summary>
        private readonly IQuoteService _quoteService;

        /// <summary>
        /// Validador de datos de entrada para QuoteRequestDto.
        /// </summary>
        private readonly IValidator<QuoteRequestDto> _validator;

        /// <summary>
        /// Constructor que inyecta las dependencias necesarias:
        /// - IQuoteService: lógica de dominio para cotizaciones.
        /// - IValidator&lt;QuoteRequestDto&gt;: verifica las reglas de negocio antes de procesar.
        /// </summary>
        /// <param name="quoteService">Instancia de servicio de cotizaciones.</param>
        /// <param name="validator">Instancia de validador de DTO.</param>
        public QuotesController(
            IQuoteService quoteService,
            IValidator<QuoteRequestDto> validator)
        {
            _quoteService = quoteService;
            _validator    = validator;
        }

        /// <summary>
        /// POST api/quotes/calculate
        /// Calcula la cuota mensual de una cotización sin persistirla.
        /// </summary>
        /// <param name="request">DTO con los datos de la cotización: cliente, precio, enganche, plazo, residual y tasa.</param>
        /// <returns>
        /// 200 OK con un QuoteResponseDto que incluye el Id generado, el monto de la cuota mensual y pago total del contrato,
        /// o 400 BadRequest con los errores de validación.
        /// </returns>
        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] QuoteRequestDto request)
        {
            ValidationResult result = _validator.Validate(request);
            if (!result.IsValid)
            {
                // Devuelve errores detallados si la validación falla
                return BadRequest(result.Errors);
            }

            var response = _quoteService.CalculateQuote(request);
            return Ok(response);
        }

        /// <summary>
        /// POST api/quotes
        /// Persiste una nueva cotización en el repositorio en memoria.
        /// </summary>
        /// <param name="request">DTO con los datos de la cotización a guardar.</param>
        /// <returns>
        /// 201 Created con la ruta para recuperar cotizaciones por cliente,
        /// o 400 BadRequest con los errores de validación.
        /// </returns>
        [HttpPost]
        public IActionResult Save([FromBody] QuoteRequestDto request)
        {
            ValidationResult result = _validator.Validate(request);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            _quoteService.SaveQuote(request);

            // Indica la ubicación del recurso para consultar por cliente
            return CreatedAtAction(
                nameof(GetByClient),
                new { clientName = request.ClientName },
                null);
        }

        /// <summary>
        /// GET api/quotes/{clientName}
        /// Obtiene todas las cotizaciones asociadas a un cliente.
        /// </summary>
        /// <param name="clientName">Nombre del cliente cuyas cotizaciones se solicitan.</param>
        /// <returns>
        /// 200 OK con una lista de QuoteResponseDto;
        /// lista vacía si no hay cotizaciones para ese cliente.
        /// </returns>
        [HttpGet("{clientName}")]
        public IActionResult GetByClient(string clientName)
        {
            var list = _quoteService.GetQuotesByClient(clientName);
            return Ok(list);
        }
    }
}
