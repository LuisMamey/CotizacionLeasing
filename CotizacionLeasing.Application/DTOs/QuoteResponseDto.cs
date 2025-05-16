using System;

namespace CotizacionLeasing.Application.DTOs
{
    /// <summary>
    /// DTO con el resultado de una cotización: su Id y la cuota mensual calculada.
    /// </summary>
    public class QuoteResponseDto
    {
        /// <summary>
        /// Identificador único de la cotización.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del cliente asociado a esta cotización.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Monto de la cuota mensual calculada.
        /// </summary>
        public decimal MonthlyPayment { get; set; }

        /// <summary>
        /// Monto total que pagara el cliente (cuota mensual * plazo en meses).
        /// </summary>
        public decimal TotalPayment { get; set; }
    }
}
