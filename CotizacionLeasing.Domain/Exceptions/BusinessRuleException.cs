using System;

namespace CotizacionLeasing.Domain.Exceptions
{
    /// <summary>
    /// Excepción personalizada que se lanza cuando una regla de negocio
    /// definida en el dominio no se cumple.
    /// </summary>
    public class BusinessRuleException : Exception
    {
        /// <summary>
        /// Crea una nueva instancia de <see cref="BusinessRuleException"/>
        /// con un mensaje descriptivo de la regla de negocio que falló.
        /// </summary>
        /// <param name="message">
        /// Mensaje que explica la regla de negocio violada.
        /// </param>
        public BusinessRuleException(string message)
            : base(message)
        { }
    }
}
