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

## Datos de referencia (seed)

Despues de aplicar las migraciones (incluida `AddTenantCodeAndType` cuando exista en el proyecto), el comando `seed` es idempotente y carga:

- Permisos, roles y `RolePermissions` (`RbacCatalog`)
- Tenant de plataforma (`Code = platform`, `Type = PLATFORM`) via `PlatformTenantCatalog`

```bash
dotnet run --project src/TalyerApp.Api/TalyerApp.Api.csproj -- seed
```

Orden recomendado en un entorno nuevo:

1. `dotnet ef database update` (ver seccion Entity Framework Core)
2. `dotnet run ... -- seed`
3. Bootstrap del super usuario (ver seccion siguiente)

## Bootstrap platform admin

Crea en la base de datos el primer usuario operativo con rol `platform_admin` en el tenant `platform`, enlazado al usuario de Keycloak mediante `ExternalIdentity` (`provider` + `sub`).

**Prerrequisitos:** usuario ya creado en Keycloak; migraciones aplicadas; `seed` ejecutado.

### Development

1. Copiar la plantilla (el archivo real esta en `.gitignore` y no se commitea):

```bash
cp src/TalyerApp.Api/usersecrets.example.json src/TalyerApp.Api/usersecrets.json
```

2. Completar `Bootstrap:PlatformAdmin` en `usersecrets.json`. El campo `ProviderUserId` debe ser el **ID de usuario** de Keycloak (claim `sub` del JWT). En la consola de administracion: realm → Users → usuario → campo **ID**.

3. Los demas campos deben coincidir con el perfil y los claims del token (`email`, `preferred_username`, `given_name`, `family_name`).

4. Ejecutar (con `ASPNETCORE_ENVIRONMENT=Development`, p. ej. perfil por defecto de `launchSettings.json`):

```bash
dotnet run --project src/TalyerApp.Api/TalyerApp.Api.csproj -- bootstrap-platform-admin
```

Las variables de entorno `Bootstrap__PlatformAdmin__*` tienen prioridad sobre `usersecrets.json` si ambas estan definidas.

### Production

No se carga `usersecrets.json`. Definir variables de entorno en el host o contenedor:

| Variable | Obligatorio | Descripcion |
|----------|-------------|-------------|
| `Bootstrap__PlatformAdmin__ProviderUserId` | Si | UUID `sub` de Keycloak |
| `Bootstrap__PlatformAdmin__Email` | Si | Claim `email` |
| `Bootstrap__PlatformAdmin__Username` | Si | Claim `preferred_username` |
| `Bootstrap__PlatformAdmin__FirstName` | Si | Claim `given_name` |
| `Bootstrap__PlatformAdmin__LastName` | Si | Claim `family_name` |
| `Bootstrap__PlatformAdmin__Provider` | No | Default: `keycloak` |

Ejemplo en Docker Compose:

```yaml
environment:
  ASPNETCORE_ENVIRONMENT: Production
  Bootstrap__PlatformAdmin__ProviderUserId: "<sub-keycloak>"
  Bootstrap__PlatformAdmin__Email: "admin@example.com"
  Bootstrap__PlatformAdmin__Username: "platform.admin"
  Bootstrap__PlatformAdmin__FirstName: "Platform"
  Bootstrap__PlatformAdmin__LastName: "Admin"
```

Ejecutar el mismo comando bootstrap en el job o contenedor de inicializacion acordado.

El comando es idempotente: puede ejecutarse de nuevo sin duplicar usuario ni asignacion.

## Solicitud de membresia e invitacion

Flujo de alta de un taller (tenant `CUSTOMER` + `tenant_admin`). El formulario **no** crea usuario en Keycloak. El `platform_admin` aprueba y ahi se crea la cuenta en el IdP (la invitacion **es** el registro: el interesado define contrasena con el mail de Keycloak).

Cualquiera puede solicitar membresia (demo / MVP, sin pago). El filtro es la aprobacion del `platform_admin`.

### Development

Completar en `usersecrets.json` (ver `usersecrets.example.json`):

- `Keycloak:Admin`: client confidential de servicio (`talyer-api-admin`) con `manage-users` en el realm `talyer-realm`.
- `Membership:ConfirmEmailBaseUrl`: origen de la API o del front (el mail de verificacion Talyer se **loguea** en consola; no hay SMTP en este MVP).

En Keycloak: Clients → Create → `talyer-api-admin`, Client authentication ON, Service accounts roles → realm-management → `manage-users`.

### Endpoints

Publicos (sin JWT):

- `POST /membership-requests`
- `POST /membership-requests/confirm-email` body `{ "token": "..." }` (el token sale en el log de la API)

Autenticados (OAuth en Scalar, usuario `platform_admin`):

- `GET /membership-requests?status=PendingReview`
- `POST /membership-requests/{id}/approve`
- `POST /membership-requests/{id}/reject`

Probar en http://localhost:5066/talyer-app-api

Si el save en Talyer falla despues de crear el usuario en Keycloak, la API intenta **borrar** ese usuario en Keycloak.

Un email = un usuario Talyer en este MVP (`Users.Email` unique). Nombre de empresa duplicado (`Tenant.Description`) rechaza la solicitud.

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
