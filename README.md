# Talyer App

API backend para Talyer App, construida con ASP.NET Core y organizada en capas.

## Requisitos

- .NET SDK 10
- PostgreSQL
- Visual Studio, Visual Studio Code o cualquier editor compatible con .NET

## Configuracion inicial

Desde la raiz del repositorio, restaura las herramientas locales del CLI:

```bash
dotnet tool restore
```

La herramienta `dotnet-ef` se administra localmente mediante `dotnet-tools.json` y se restaura con el comando anterior.

Restaura las dependencias de la solucion:

```bash
dotnet restore
```

## Compilar el proyecto

```bash
dotnet build TalyerApp.slnx
```

## Ejecutar la API

```bash
dotnet run --project src/TalyerApp.Api/TalyerApp.Api.csproj
```

Con la configuracion actual, la API estara disponible en:

- HTTP: http://localhost:5066
- HTTPS: https://localhost:7129

## Documentacion de la API

En el entorno de desarrollo se habilitan OpenAPI y Scalar:

- OpenAPI: http://localhost:5066/openapi/v1.json
- Scalar: http://localhost:5066/talyer-app-api

Tambien se puede ejecutar directamente el perfil HTTPS:

```bash
dotnet run --project src/TalyerApp.Api/TalyerApp.Api.csproj --launch-profile https
```

## Entity Framework Core

Despues de ejecutar `dotnet tool restore`, se pueden utilizar los comandos de Entity Framework Core desde la raiz del repositorio.

Crear una migracion:

```bash
dotnet ef migrations add InitialCreate \
  --project src/TalyerApp.Infrastructure/TalyerApp.Infrastructure.csproj \
  --startup-project src/TalyerApp.Api/TalyerApp.Api.csproj
```

Aplicar las migraciones a la base de datos:

```bash
dotnet ef database update \
  --project src/TalyerApp.Infrastructure/TalyerApp.Infrastructure.csproj \
  --startup-project src/TalyerApp.Api/TalyerApp.Api.csproj
```

> La cadena de conexion y la configuracion de PostgreSQL deben definirse antes de ejecutar migraciones o actualizar la base de datos.

## Estructura del proyecto

```text
src/
├── TalyerApp.Api/
│   └── Punto de entrada y configuracion de la API
├── TalyerApp.Application/
│   ├── DTOs
│   └── Interfaces de aplicacion y repositorios
├── TalyerApp.Domain/
│   ├── Entidades
│   ├── Value Objects
│   └── Result y errores de dominio
└── TalyerApp.Infrastructure/
    └── Persistencia y contextos de Entity Framework Core
```

## Estado actual

La API incluye actualmente:

- Configuracion de OpenAPI.
- Documentacion interactiva mediante Scalar.
- Redireccion HTTPS.
- Endpoint de ejemplo `GET /weatherforecast`.
- Capas Domain, Application, Infrastructure y API.
- Persistencia preparada para PostgreSQL mediante Entity Framework Core.

## Licencia

Este proyecto no tiene una licencia definida actualmente.
