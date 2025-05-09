using System.Collections.Generic;
using CotizacionLeasing.Application.DTOs;

namespace CotizacionLeasing.Application.Interfaces
{
    /// <summary>
    /// Define el contrato para el servicio de cotizaciones, incluyendo
    /// cálculo, persistencia y consulta por cliente.
    /// </summary>
    public interface IQuoteService
    {
        /// <summary>
        /// Calcula la cuota mensual de una cotización sin guardarla en el repositorio.
        /// </summary>
        /// <param name="request">
        /// DTO con los datos necesarios: nombre de cliente, precio, enganche,
        /// plazo en meses, residual y tasa anual.
        /// </param>
        /// <returns>
        /// Un <see cref="QuoteResponseDto"/> que incluye el Id generado de la cotización
        /// y el monto de la cuota mensual calculada.
        /// </returns>
        QuoteResponseDto CalculateQuote(QuoteRequestDto request);

        /// <summary>
        /// Persiste una nueva cotización en el repositorio configurado.
        /// </summary>
        /// <param name="request">
        /// DTO con los datos de la cotización a guardar: nombre de cliente,
        /// precio, enganche, plazo, residual y tasa anual.
        /// </param>
        void SaveQuote(QuoteRequestDto request);

        /// <summary>
        /// Recupera todas las cotizaciones asociadas a un cliente específico.
        /// </summary>
        /// <param name="clientName">
        /// Nombre del cliente cuyas cotizaciones se deben obtener.
        /// </param>
        /// <returns>
        /// Una colección de <see cref="QuoteResponseDto"/> con los resultados de
        /// las cotizaciones encontradas; puede estar vacía si no hay registros.
        /// </returns>
        IEnumerable<QuoteResponseDto> GetQuotesByClient(string clientName);
    }
}
