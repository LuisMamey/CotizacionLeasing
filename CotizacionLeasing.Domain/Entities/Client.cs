using System;

namespace CotizacionLeasing.Domain.Entities
{
    /// <summary>
    /// Representa un cliente en el dominio de cotizaciones.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Identificador único del cliente, generado automáticamente.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Nombre completo del cliente. No puede ser nulo, vacío o contener solo espacios.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="Client"/> validando el nombre.
        /// </summary>
        /// <param name="name">
        /// Nombre del cliente. Si es null, vacío o whitespace, lanza <see cref="ArgumentException"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Se lanza cuando <paramref name="name"/> es nulo, vacío o solo espacios.
        /// </exception>
        public Client(string name)
        {
            // Validación de entrada: el nombre es obligatorio
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del cliente es obligatorio.", nameof(name));

            // Asigna un nuevo GUID como Id
            Id = Guid.NewGuid();

            // Asigna el nombre validado
            Name = name;
        }
    }
}
