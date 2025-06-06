using System;
using CotizacionLeasing.Domain.Exceptions;

namespace CotizacionLeasing.Domain.Entities
{
    /// <summary>
    /// Entidad que representa una cotización de arrendamiento.
    /// Aplica las reglas de negocio según el plazo, enganche y residual,
    /// y calcula la cuota mensual usando la fórmula PMT de Excel.
    /// </summary>
    public class Quote
    {
        /// <summary>
        /// Identificador único de la cotización, generado automáticamente.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Cliente asociado a esta cotización.
        /// </summary>
        public Client Client { get; }

        /// <summary>
        /// Precio total del activo a cotizar.
        /// </summary>
        public decimal Price { get; }

        /// <summary>
        /// Monto de enganche o pago inicial.
        /// </summary>
        public decimal DownPayment { get; }

        /// <summary>
        /// Plazo de financiamiento en meses.
        /// </summary>
        public int TermMonths { get; }

        /// <summary>
        /// Valor residual al finalizar el contrato.
        /// </summary>
        public decimal Residual { get; }

        /// <summary>
        /// Tasa de interés anual expresada en forma decimal (por ejemplo 0.12 = 12%).
        /// </summary>
        public double AnnualRate { get; }

        /// <summary>
        /// Cuota mensual resultante del cálculo PMT.
        /// </summary>
        public decimal MonthlyPayment { get; }

        /// <summary>
        /// Monto total que se pagara al final del contrato.
        /// cuota mensual * plazo en meses.
        /// </summary>
        public decimal TotalPayment { get; }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="Quote"/>, aplica
        /// las reglas de negocio y calcula la cuota mensual.
        /// </summary>
        /// <param name="client">Cliente que solicita la cotización. Debe estar instanciado.</param>
        /// <param name="price">Precio total del activo. Debe ser mayor que cero.</param>
        /// <param name="downPayment">Enganche inicial. Se valida según el plazo.</param>
        /// <param name="termMonths">Plazo en meses. Debe ser positivo.</param>
        /// <param name="residual">Valor residual. No puede exceder el 30% del precio.</param>
        /// <param name="annualRate">Tasa anual de interés. Debe ser mayor que cero.</param>
        /// <exception cref="BusinessRuleException">
        /// Se lanza si alguna regla de negocio no se cumple:
        /// - Residual > 30% del precio.
        /// - Enganche mínimo no alcanzado según el plazo:
        ///   * 12 meses → ≥ 10%
        ///   * 13–24 meses → ≥ 7.5%
        ///   * ≥ 25 meses  → ≥ 5%
        /// </exception>
        public Quote(
            Client client,
            decimal price,
            decimal downPayment,
            int termMonths,
            decimal residual,
            double annualRate)
        {
            // 1. Validar que el residual no supere el 30% del precio
            if (residual > price * 0.30m)
                throw new BusinessRuleException("El residual no debe superar el 30% del precio.");

            // 2. Validar enganche mínimo según plazo
            if (termMonths == 12 && downPayment < price * 0.10m)
                throw new BusinessRuleException("Para 12 meses, enganche mínimo de 10%.");
            if (termMonths is >= 13 and <= 24 && downPayment < price * 0.075m)
                throw new BusinessRuleException("Para 13–24 meses, enganche mínimo de 7.5%.");
            if (termMonths >= 25 && downPayment < price * 0.05m)
                throw new BusinessRuleException("Para 25 meses o más, enganche mínimo de 5%.");

            // Asignación de propiedades
            Id             = Guid.NewGuid();
            Client         = client;
            Price          = price;
            DownPayment    = downPayment;
            TermMonths     = termMonths;
            Residual       = residual;
            AnnualRate     = annualRate;

            // Cálculo de la cuota mensual
            MonthlyPayment = CalculateMonthlyPayment();

            //Calculo del pago total a lo largo de todo el plazo
            TotalPayment = MonthlyPayment * TermMonths;
        }

        /// <summary>
        /// Calcula la cuota mensual usando la fórmula PMT:
        /// PMT = (r * (pv + fv / (1+r)^n)) / (1 − (1+r)^(-n))
        /// donde:
        /// r  = tasa periódica (tasa anual / 12),
        /// pv = monto financiado (price - downPayment),
        /// fv = valor residual,
        /// n  = número de pagos (termMonths).
        /// </summary>
        /// <returns>Valor de la cuota mensual.</returns>
        private decimal CalculateMonthlyPayment()
        {
            double r  = AnnualRate / 12.0;
            int    n  = TermMonths;
            double pv = (double)(Price - DownPayment);
            double fv = (double)Residual;

            double numerator   = r * (pv + fv / Math.Pow(1 + r, n));
            double denominator = 1 - Math.Pow(1 + r, -n);
            double pmt         = numerator / denominator;

            return (decimal)pmt;
        }
    }
}
