using System.Collections.Generic;
using System.Linq;
using CotizacionLeasing.Application.DTOs;
using CotizacionLeasing.Application.Interfaces;
using CotizacionLeasing.Domain.Entities;
using CotizacionLeasing.Infrastructure.Interfaces;

namespace CotizacionLeasing.Application.Services
{
    /// <summary>
    /// Servicio que implementa las operaciones definidas en IQuoteService.
    /// Orquesta la construcción de entidades de dominio, el cálculo de cuota
    /// y la persistencia mediante IQuoteRepository.
    /// </summary>
    public class QuoteService : IQuoteService
    {
        /// <summary>
        /// Repositorio para persistir y recuperar entidades Quote.
        /// </summary>
        private readonly IQuoteRepository _repository;

        /// <summary>
        /// Inicializa una nueva instancia de QuoteService con el repositorio especificado.
        /// </summary>
        /// <param name="repository">Repositorio que manejará la persistencia de cotizaciones.</param>
        public QuoteService(IQuoteRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Calcula la cuota mensual de una cotización sin guardarla.
        /// </summary>
        /// <param name="request">DTO con los datos necesarios para el cálculo: nombre de cliente, precio, enganche, plazo, residual y tasa anual.</param>
        /// <returns>DTO con el identificador de la cotización y el monto de la cuota mensual calculada.</returns>
        public QuoteResponseDto CalculateQuote(QuoteRequestDto request)
        {
            // Crea la entidad Cliente validando que el nombre no esté vacío
            var client = new Client(request.ClientName);

            // Crea la entidad Quote, aplicando todas las reglas de negocio y calculando la cuota
            var quote = new Quote(
                client,
                request.Price,
                request.DownPayment,
                request.TermMonths,
                request.Residual,
                request.AnnualRate);

            // Mapea la entidad Quote al DTO de respuesta
            return new QuoteResponseDto
            {
                Id = quote.Id,
                ClientName = client.Name,
                MonthlyPayment = quote.MonthlyPayment
            };
        }

        /// <summary>
        /// Persiste una nueva cotización en el almacenamiento configurado.
        /// </summary>
        /// <param name="request">DTO con los datos de la cotización a guardar.</param>
        public void SaveQuote(QuoteRequestDto request)
        {
            // Crea la entidad Cliente y la cotización para persistirla
            var client = new Client(request.ClientName);
            var quote = new Quote(
                client,
                request.Price,
                request.DownPayment,
                request.TermMonths,
                request.Residual,
                request.AnnualRate);

            // Llama al repositorio para almacenar la entidad Quote
            _repository.Add(quote);
        }

        /// <summary>
        /// Recupera todas las cotizaciones asociadas a un cliente específico.
        /// </summary>
        /// <param name="clientName">Nombre del cliente cuyas cotizaciones se desean obtener.</param>
        /// <returns>Lista de DTOs con los resultados de las cotizaciones encontradas.</returns>
        public IEnumerable<QuoteResponseDto> GetQuotesByClient(string clientName)
        {
            // Obtiene las entidades Quote del repositorio y las convierte en DTOs
            return _repository.GetByClientName(clientName)
                .Select(q => new QuoteResponseDto
                {
                    Id = q.Id,
                    ClientName = q.Client.Name,
                    MonthlyPayment = q.MonthlyPayment
                });
        }
    }
}
