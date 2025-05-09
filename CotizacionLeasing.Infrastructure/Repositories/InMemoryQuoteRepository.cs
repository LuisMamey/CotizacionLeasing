using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CotizacionLeasing.Domain.Entities;
using CotizacionLeasing.Infrastructure.Interfaces;

namespace CotizacionLeasing.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación en memoria de <see cref="IQuoteRepository"/>.
    /// Útil para pruebas y prototipos sin base de datos real.
    /// </summary>
    public class InMemoryQuoteRepository : IQuoteRepository
    {
        /// <summary>
        /// Almacena internamente las cotizaciones de forma thread-safe.
        /// </summary>
        private readonly ConcurrentBag<Quote> _storage = new();

        /// <summary>
        /// Agrega una nueva cotización al almacenamiento en memoria.
        /// </summary>
        /// <param name="quote">Instancia de <see cref="Quote"/> a guardar.</param>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="quote"/> es null.
        /// </exception>
        public void Add(Quote quote)
        {
            if (quote == null) 
                throw new ArgumentNullException(nameof(quote));

            _storage.Add(quote);
        }

        /// <summary>
        /// Recupera todas las cotizaciones asociadas a un nombre de cliente.
        /// </summary>
        /// <param name="clientName">
        /// Nombre del cliente que se usará como filtro (case-insensitive).
        /// </param>
        /// <returns>
        /// Colección de <see cref="Quote"/> que coinciden con el cliente,
        /// o una colección vacía si no se proporciona un nombre válido.
        /// </returns>
        public IEnumerable<Quote> GetByClientName(string clientName)
        {
            if (string.IsNullOrWhiteSpace(clientName))
                return Enumerable.Empty<Quote>();

            return _storage
                .Where(q => q.Client.Name
                    .Equals(clientName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
