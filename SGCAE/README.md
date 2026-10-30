# SGCAE – Sistema de Gestión y Control de Acceso de Empleados

Backend con **ASP.NET Core 8 Web API** y **SQL Server** (Entity Framework Core) para administrar empleados,
credenciales, puntos de acceso y el historial de entradas y salidas de una organización.

Aplica **Clean Architecture**, **DDD** y **CQRS** (MediatR).

## Arquitectura

```
SGCAE.Domain          → Reglas del negocio. No depende de nada.
SGCAE.Application     → Casos de uso (Commands/Queries), validaciones e interfaces.  → Domain
SGCAE.Infrastructure  → EF Core, SQL Server, repositorios y consultas.              → Application
SGCAE.API             → Controllers REST, Swagger y arranque.                       → Application, Infrastructure
SGCAE.Domain.Tests    → Pruebas unitarias de las reglas del dominio.                → Domain
```

La regla de dependencia: las capas externas conocen a las internas, nunca al revés.

### DDD (SGCAE.Domain)
- **Entidades:** `Employee`, `Credential`, `AccessPoint`, `AccessLog`.
- **Objeto de valor:** `Email` (siempre válido y normalizado).
- **Invariantes** dentro de las propias entidades (constructores/métodos estáticos y métodos de comportamiento):
  - Un empleado inactivo no puede recibir credenciales ni acceder.
  - Una credencial bloqueada o revocada no puede usarse; una revocada no se reactiva.
  - Un punto de acceso deshabilitado rechaza todo acceso.
  - Todo intento queda registrado, incluso los rechazados, con su motivo.

### CQRS (SGCAE.Application)
- **Commands** (modifican): `CreateEmployee`, `UpdateEmployee`, `DeactivateEmployee`, `AssignCredential`,
  `Block/Reactivate/RevokeCredential`, `CreateAccessPoint`, `Enable/DisableAccessPoint`, `RegisterAccess`.
  Usan **repositorios** + **UnitOfWork** sobre entidades del dominio.
- **Queries** (solo leen): `GetEmployeeById`, `GetEmployees`, `GetCredentialById`, `GetCredentialsByEmployee`,
  `GetAccessPoints`, `GetAccessPointById`, `SearchAccessLogs`.
  Usan interfaces `I...Queries` con `AsNoTracking` y devuelven **DTOs**.
- Un `ValidationBehavior` (FluentValidation) valida cada solicitud antes de ejecutarla.

## Requisitos
- .NET 8 SDK
- SQL Server (LocalDB, Express, Developer o Docker)
- Herramienta de EF: `dotnet tool install --global dotnet-ef --version 8.0.11`

## Puesta en marcha

1. **Cadena de conexión:** edita `SGCAE.API/appsettings.json`. Por defecto usa LocalDB:
   `Server=(localdb)\MSSQLLocalDB;Database=SGCAE;Trusted_Connection=True;TrustServerCertificate=True;`

   Otras opciones:
   - SQL Express: `Server=.\SQLEXPRESS;Database=SGCAE;Trusted_Connection=True;TrustServerCertificate=True;`
   - Docker: `Server=localhost,1433;Database=SGCAE;User Id=sa;Password=Clave_Segura123;TrustServerCertificate=True;`

2. **Compilar:** `dotnet build`

3. **Crear la migración** (una sola vez, desde la carpeta de la solución):
   ```
   dotnet ef migrations add InitialCreate --project SGCAE.Infrastructure --startup-project SGCAE.API
   ```

4. **Ejecutar la API** (en modo Development crea la base de datos y las tablas automáticamente):
   ```
   dotnet run --project SGCAE.API --launch-profile http
   ```
5. Abrir **http://localhost:5052/swagger**

6. **Pruebas unitarias:** `dotnet test`

## Endpoints

| Requerimiento | Método y ruta |
|---|---|
| RF-01 Empleados | `POST /api/employees`, `GET /api/employees`, `GET /api/employees/{id}`, `PUT /api/employees/{id}`, `POST /api/employees/{id}/deactivate` |
| RF-02 Credenciales | `POST /api/credentials`, `GET /api/credentials/{id}`, `GET /api/employees/{employeeId}/credentials`, `POST /api/credentials/{id}/block`, `/reactivate`, `/revoke` |
| RF-03 Entradas y salidas | `POST /api/access` |
| RF-04 Historial | `GET /api/access/history` (filtros: `employeeId`, `from`, `to`, `accessPointId`, `result`, `page`, `pageSize`) |
| RF-06 Puntos de acceso | `POST /api/access-points`, `GET /api/access-points`, `GET /api/access-points/{id}`, `POST /api/access-points/{id}/enable`, `/disable` |

Los errores se devuelven como `ProblemDetails`: 400 (validación o regla de negocio), 404 (no existe), 409 (duplicado).

## Escenario de prueba sugerido (Swagger)

1. `POST /api/employees`
   ```json
   { "identification": "1001", "firstName": "Ana", "lastName": "Pérez",
     "email": "ana@empresa.com", "position": "Operaria", "area": "Bodega" }
   ```
   Copia el `id` devuelto.
2. `POST /api/credentials` → `{ "employeeId": "<id empleado>", "code": "CRD-001" }`
3. `POST /api/access-points` → `{ "name": "Recepción", "description": "Entrada principal" }`. Copia el `id`.
4. `POST /api/access` → `{ "credentialCode": "CRD-001", "accessPointId": "<id punto>", "movement": "Entry" }`
   → `result: "Authorized"`.
5. Casos de rechazo (cada uno queda registrado):
   - `POST /api/credentials/{id}/block` y repetir el paso 4 → `CredentialBlocked`.
   - `POST /api/credentials/{id}/reactivate`, luego `POST /api/employees/{id}/deactivate` y repetir → `EmployeeInactive`.
   - `POST /api/access-points/{id}/disable` y repetir con otro empleado activo → `AccessPointDisabled`.
   - Usar un código inexistente (`"XXX"`) → `CredentialNotFound`.
6. `GET /api/access/history?page=1&pageSize=5` y probar los filtros (`result=Rejected`, `employeeId=...`).
7. Verificar en SSMS / Azure Data Studio las tablas `Employees`, `Credentials`, `AccessPoints`, `AccessLogs`.

## Estructura de la base de datos
`Employees`, `Credentials`, `AccessPoints`, `AccessLogs` (+ `__EFMigrationsHistory`), con índices únicos en
identificación, correo, código de credencial y nombre del punto de acceso.
