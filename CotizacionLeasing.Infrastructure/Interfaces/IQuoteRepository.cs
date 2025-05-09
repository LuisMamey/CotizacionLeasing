using System.Collections.Generic;
using CotizacionLeasing.Domain.Entities;

namespace CotizacionLeasing.Infrastructure.Interfaces
{
    /// <summary>
    /// Contrato para el repositorio de cotizaciones.
    /// Define operaciones de persistencia y consulta para la entidad Quote.
    /// </summary>
    public interface IQuoteRepository
    {
        /// <summary>
        /// Agrega una nueva cotización al almacenamiento.
        /// </summary>
        /// <param name="quote">Entidad de cotización a guardar.</param>
        void Add(Quote quote);

        /// <summary>
        /// Obtiene todas las cotizaciones asociadas a un nombre de cliente.
        /// </summary>
        /// <param name="clientName">Nombre de cliente a filtrar.</param>
        /// <returns>Colección de cotizaciones encontradas.</returns>
        IEnumerable<Quote> GetByClientName(string clientName);
    }
}
