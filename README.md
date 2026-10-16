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

Auth0 se configura bajo `Auth0` (`appsettings.json`, `appsettings.{Environment}.json` o `usersecrets.json`). **ValidateOnStart**: la API no arranca si falta configuracion obligatoria. `BaseUrl` y `Realm` se definen una vez en la raiz (`Auth0:BaseUrl`, `Auth0:Realm`) y aplican a JWT y al cliente Admin HTTP, salvo override en `Auth0:Api` o `Auth0:Admin`.

| Clave | Obligatorio | Descripcion |
|-------|-------------|-------------|
| `Auth0__BaseUrl` | Si | URL base del servidor Auth0 (p. ej. `http://localhost:8080`) |
| `Auth0__Realm` | Si | Nombre del realm (p. ej. `talyer-realm`) |
| `Auth0__Api__Audience` | Si | Audiencia del resource server |
| `Auth0__Api__RequireHttpsMetadata` | Si | `true` o `false` explicito (`false` en Development, `true` en Production via `appsettings.Production.json`) |
| `Auth0__Api__Authority` | Condicional | Authority JWT; si no se define, se usa `{BaseUrl}/realms/{Realm}` (raiz o override en Api) |
| `Auth0__Api__BaseUrl` | No | Override opcional de `Auth0:BaseUrl` solo para JWT |
| `Auth0__Api__Realm` | No | Override opcional de `Auth0:Realm` solo para JWT |
| `Auth0__Admin__ClientId` | Si | Client confidential de servicio (`talyer-api-admin`) |
| `Auth0__Admin__ClientSecret` | Si | Secret del client Admin (solo `usersecrets` / variables de entorno, no commitear) |
| `Auth0__Admin__BaseUrl` | No | Override opcional de `Auth0:BaseUrl` solo para Admin API |
| `Auth0__Admin__Realm` | No | Override opcional de `Auth0:Realm` solo para Admin API |
| `Auth0__Docs__ScalarRoute` | Si | Ruta de Scalar (debe empezar con `/`) |
| `Auth0__Docs__OAuthClientId` | Si | Client publico para la documentacion |
| `Auth0__Docs__OAuthScopes` | Si | Al menos un scope (p. ej. `openid`, `profile`, `email`) |
| `Talyer__Session__Issuer` | Si | Issuer del JWT de sesion Talyer (`appsettings.json`) |
| `Talyer__Session__Audience` | Si | Audience del JWT de sesion Talyer |
| `Talyer__Session__LifetimeMinutes` | Si | Vida del JWT Talyer (p. ej. 480) |
| `Talyer__Session__SigningKey` | Si | HMAC del JWT Talyer (min. 32 bytes; solo `usersecrets` / entorno) |
| `Smtp__Host` | Si | Host SMTP |
| `Smtp__Port` | Si | Puerto SMTP (p. ej. 587) |
| `Smtp__User` | Si | Usuario SMTP |
| `Smtp__Password` | Si | Password SMTP (solo `usersecrets` / variables de entorno) |
| `Smtp__From` | Si | Remitente (p. ej. `noreply@example.com`) |
| `Smtp__UseSsl` | Si | `true` = STARTTLS |

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

> La cadena de conexion (`ConnectionStrings:DefaultConnection`) no se commitea. En Development va en `usersecrets.json` (ver `usersecrets.example.json`). En Production usar `ConnectionStrings__DefaultConnection`. Definirla antes de migraciones o `dotnet run`.

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

Crea (o reutiliza) el primer usuario operativo en Talyer con rol `platform_owner` en el tenant `platform`. **No** pide el `sub` de Auth0 ni escribe `ExternalIdentity`. El enlace al IdP ocurre en el primer Magic Link: `POST /session` busca el `User` por email e inserta `ExternalIdentity`.

**Prerrequisitos:** migraciones aplicadas; `seed` ejecutado. El email de Auth0 Passwordless debe coincidir con `Bootstrap:PlatformOwner:Email`.

### Development

1. Copiar la plantilla (el archivo real esta en `.gitignore` y no se commitea):

```bash
cp src/TalyerApp.Api/usersecrets.example.json src/TalyerApp.Api/usersecrets.json
```

2. Completar `ConnectionStrings:DefaultConnection`, `Talyer:Session:SigningKey` (minimo 32 caracteres) y `Bootstrap:PlatformOwner` (`Email`, `Username`, `FirstName`, `LastName`) en `usersecrets.json`.

3. Ejecutar (con `ASPNETCORE_ENVIRONMENT=Development`, p. ej. perfil por defecto de `launchSettings.json`):

```bash
dotnet run --project src/TalyerApp.Api/TalyerApp.Api.csproj -- bootstrap-platform-owner
```

4. Login: Magic Link de Auth0 con ese email → `POST /session` (Bearer Auth0) → JWT Talyer para el resto de la API.

Las variables de entorno `Bootstrap__PlatformOwner__*` tienen prioridad sobre `usersecrets.json` si ambas estan definidas.

### Production

No se carga `usersecrets.json`. Definir variables de entorno en el host o contenedor:

| Variable | Obligatorio | Descripcion |
|----------|-------------|-------------|
| `ConnectionStrings__DefaultConnection` | Si | Cadena PostgreSQL (incluye password) |
| `Talyer__Session__SigningKey` | Si | Clave HMAC del JWT Talyer (min. 32 bytes) |
| `Bootstrap__PlatformOwner__Email` | Si | Email del platform owner (mismo que Auth0) |
| `Bootstrap__PlatformOwner__Username` | Si | Username local |
| `Bootstrap__PlatformOwner__FirstName` | Si | Nombre |
| `Bootstrap__PlatformOwner__LastName` | Si | Apellido |

Ejemplo en Docker Compose:

```yaml
environment:
  ASPNETCORE_ENVIRONMENT: Production
  ConnectionStrings__DefaultConnection: "Host=postgres;Port=5432;Database=talyer;Username=postgres;Password=<secret>"
  Talyer__Session__SigningKey: "<at-least-32-character-secret>"
  Bootstrap__PlatformOwner__Email: "admin@example.com"
  Bootstrap__PlatformOwner__Username: "platform.admin"
  Bootstrap__PlatformOwner__FirstName: "Platform"
  Bootstrap__PlatformOwner__LastName: "Admin"
```

Ejecutar el mismo comando bootstrap en el job o contenedor de inicializacion acordado.

El comando es idempotente: puede ejecutarse de nuevo sin duplicar usuario ni asignacion.

## Solicitud de membresia e invitacion

Flujo de alta de un taller (tenant `CUSTOMER` + `tenant_owner`). El formulario **no** crea usuario en Auth0. El `platform_owner` aprueba y ahi se crea la cuenta en el IdP. El login posterior es passwordless (Magic Link de Auth0), no un mail de set-password.

Cualquiera puede solicitar membresia (demo / MVP, sin pago). El filtro es la aprobacion del `platform_owner`.

### Development

Completar en `usersecrets.json` (ver `usersecrets.example.json`):

- `ConnectionStrings:DefaultConnection`: PostgreSQL local (el password no va en `appsettings.json`).
- `Auth0:Admin`: client confidential de servicio (`talyer-api-admin`) con `manage-users` en el realm `talyer-realm`.
- `Membership:ConfirmEmailBaseUrl`: origen de la API (el link del mail apunta a `GET /membership-requests/confirm-email?token=`).
- `Smtp`: `Host`, `Port`, `User`, `Password`, `From`, `UseSsl` (obligatorio; secretos en `usersecrets.json`).

Cuerpos HTML editables en `src/TalyerApp.Api/Resources/Emails/` (`confirm-request.html`, `email-confirmed.html`, paginas de resultado). Placeholders: `{{ConfirmUrl}}`, `{{FirstName}}`, `{{LastName}}`, `{{CompanyName}}`, `{{Message}}`, `{{LifetimeHours}}`.

En Auth0: Clients → Create → `talyer-api-admin`, Client authentication ON, Service accounts roles → realm-management → `manage-users`.

La aprobacion crea el usuario en Auth0 y persiste tenant/usuario/`ExternalIdentity` en Talyer. Si falla la persistencia, se elimina el usuario en Auth0 y la solicitud sigue en `PendingReview`. El interesado entra luego con Magic Link y `POST /session`.

### Endpoints

Publicos (sin JWT):

- `POST /membership-requests` (envia mail HTML con el link de confirmacion; si SMTP falla responde **500** `MembershipRequest.EmailDeliveryFailed` y la solicitud queda `PendingEmailVerification`)
- `GET /membership-requests/confirm-email?token=...` (un clic; respuesta HTML). Confirma y envia el mail de exito en la misma transaccion. Si el mail de exito falla: rollback (`PendingEmailVerification`), **500** HTML, detalle en logs; el mismo link se puede reintentar.

Sesion:

- `POST /session` (Bearer Auth0: `sub` + `email`). Devuelve JWT Talyer y el perfil de DB. Si no hay `User` con ese email responde **403**. No crea usuarios.

Autenticados (JWT Talyer, usuario `platform_owner`):

- `GET /membership-requests?status=PendingReview`
- `POST /membership-requests/{id}/approve`
- `POST /membership-requests/{id}/reject`

En Scalar: OAuth2 (Auth0) para `/session`, luego pegar el access token Talyer (`TalyerJwt`) para el resto.

Probar en http://localhost:5066/talyer-app-api

Si falla el save en Talyer tras crear el usuario en Auth0, la API intenta **borrar** ese usuario en Auth0 y no deja la solicitud en `Approved`.

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
