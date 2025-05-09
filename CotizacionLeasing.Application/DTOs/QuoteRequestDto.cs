using System;

namespace CotizacionLeasing.Application.DTOs
{
    /// <summary>
    /// DTO que representa los datos de entrada para solicitar una cotización.
    /// </summary>
    public class QuoteRequestDto
    {
        /// <summary>
        /// Nombre del cliente que solicita la cotización.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Precio total del activo a cotizar.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Monto de enganche o pago inicial.
        /// </summary>
        public decimal DownPayment { get; set; }

        /// <summary>
        /// Plazo de financiamiento en meses.
        /// </summary>
        public int TermMonths { get; set; }

        /// <summary>
        /// Valor residual al finalizar el contrato.
        /// </summary>
        public decimal Residual { get; set; }

        /// <summary>
        /// Tasa de interés anual (por ejemplo, 0.12 = 12%).
        /// </summary>
        public double AnnualRate { get; set; }
    }
}
