using FluentValidation;
using CotizacionLeasing.Application.DTOs;

namespace CotizacionLeasing.Application.Validators
{
    /// <summary>
    /// Validador de <see cref="QuoteRequestDto"/> que asegura
    /// que los datos de entrada cumplan con las reglas de negocio
    /// antes de procesar la cotización.
    /// </summary>
    public class QuoteRequestValidator : AbstractValidator<QuoteRequestDto>
    {
        public QuoteRequestValidator()
        {
            // 1. Nombre del cliente obligatorio
            RuleFor(x => x.ClientName)
                .NotEmpty()
                .WithMessage("El nombre del cliente es obligatorio.");

            // 2. Precio > 0
            RuleFor(x => x.Price)
                .GreaterThan(0m)
                .WithMessage("El precio debe ser mayor que cero.");

            // 3. Residual ≤ 30% del precio
            RuleFor(x => x.Residual)
                .LessThanOrEqualTo(x => x.Price * 0.30m)
                .WithMessage("El residual no debe superar el 30% del precio.");

            // 4a. Enganche mínimo 10% para 12 meses
            RuleFor(x => x.DownPayment)
                .GreaterThanOrEqualTo(x => x.Price * 0.10m)
                .When(x => x.TermMonths == 12)
                .WithMessage("Para 12 meses, enganche mínimo de 10%.");

            // 4b. Enganche mínimo 7.5% para 13–24 meses
            RuleFor(x => x.DownPayment)
                .GreaterThanOrEqualTo(x => x.Price * 0.075m)
                .When(x => x.TermMonths >= 13 && x.TermMonths <= 24)
                .WithMessage("Para 13–24 meses, enganche mínimo de 7.5%.");

            // 4c. Enganche mínimo 5% para 25 meses o más
            RuleFor(x => x.DownPayment)
                .GreaterThanOrEqualTo(x => x.Price * 0.05m)
                .When(x => x.TermMonths >= 25)
                .WithMessage("Para 25 meses o más, enganche mínimo de 5%.");

            // 5. Plazo en meses > 0
            RuleFor(x => x.TermMonths)
                .GreaterThan(0)
                .WithMessage("El plazo debe ser al menos 1 mes.");

            // 6. Tasa anual > 0
            RuleFor(x => x.AnnualRate)
                .GreaterThan(0)
                .WithMessage("La tasa anual debe ser mayor que cero.");
        }
    }
}
