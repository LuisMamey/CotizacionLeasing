Proyecto Entrevista .NET

Este repositorio contiene la solución CotizacionLeasing, implementada con Onion/Clean Architecture en C# (.NET 8.0), que permite calcular cotizaciones de arrendamiento, validar reglas de negocio y exponer una API REST.

Estructura de carpetas

CotizacionLeasing/
├── CotizacionLeasing.sln
├── CotizacionLeasing.Domain/         # Entidades y reglas de negocio
├── CotizacionLeasing.Application/    # DTOs, validación y servicios de aplicación
├── CotizacionLeasing.Infrastructure/ # Repositorios (InMemory) e interfaces
├── CotizacionLeasing.API/            # Web API (Controllers, DI, Swagger)
└── CotizacionLeasing.Tests/          # Pruebas unitarias (xUnit + Moq + FluentValidation)

Tecnologías

Plataforma: .NET 6+ (se utiliza .NET 8.0)
Lenguaje: C# 11
Validación: FluentValidation
Pruebas: xUnit, Moq
Persistencia: InMemory (ConcurrentBag)
Exposición: ASP.NET Core Web API + Swagger UI

Cómo ejecutar la aplicación

Clona este repositorio:

git clone https://github.com/LuisMamey/CotizacionLeasing.git
cd CotizacionLeasing

Restaurar paquetes y compilar:

dotnet restore
dotnet build

Iniciar la API:

dotnet run --project CotizacionLeasing.API

Abrir en tu navegador la documentación Swagger:

http://localhost:5238/swagger

Desde allí puedes probar los endpoints:

POST /api/quotes/calculate

POST /api/quotes

GET  /api/quotes/{clientName}

Cómo ejecutar los tests

Desde la raíz de la solución:

dotnet test

Verifica que todos los tests pasen (deben verse todos en verde).