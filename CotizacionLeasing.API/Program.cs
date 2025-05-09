// Program.cs
// Punto de entrada de la aplicación ASP.NET Core.
// Aquí configuramos servicios, validación, Swagger, inyección de dependencias
// y definimos el pipeline HTTP.

using FluentValidation;
using FluentValidation.AspNetCore;
using CotizacionLeasing.Application.Interfaces;
using CotizacionLeasing.Application.Services;
using CotizacionLeasing.Application.Validators;
using CotizacionLeasing.Infrastructure.Interfaces;
using CotizacionLeasing.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

//
// 1. Registrar validadores de FluentValidation desde el ensamblado
//    QuoteRequestValidator detectará automáticamente su propio validador.
// 
builder.Services
    .AddValidatorsFromAssemblyContaining<QuoteRequestValidator>();

//
// 2. Habilitar la validación automática en modelos y validación del lado cliente.
//    Esto reemplaza AddFluentValidation obsoleto.
//
builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

//
// 3. Configurar MVC y Swagger/OpenAPI para documentación interactiva.
//
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
// 4. Inyección de dependencias:
//    - Singleton: repositorio en memoria que mantiene datos entre peticiones.
//    - Scoped: servicio de cotizaciones, instancia por petición HTTP.
//
builder.Services.AddSingleton<IQuoteRepository, InMemoryQuoteRepository>();
builder.Services.AddScoped<IQuoteService, QuoteService>();

var app = builder.Build();

//
// Habilitar Swagger únicamente en entorno de desarrollo.
//
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//
// Middleware para redirigir HTTP a HTTPS.
// Si no necesitas HTTPS en local, puedes comentar esta línea.
//
app.UseHttpsRedirection();

//
// Middleware de autorización (por si más adelante incluyes AuthN/AuthZ).
//
app.UseAuthorization();

//
// Mapea los controladores a rutas HTTP (attribute routing).
//
app.MapControllers();

app.Run();
