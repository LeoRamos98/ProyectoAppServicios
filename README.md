# App Servicios

Trabajo Práctico Integrador de la Tecnicatura Universitaria en Programación de la UTN, grupo Firmat.

App Servicios es una API REST para contratar trabajos de servicio. El cliente publica qué necesita, dónde se realizará y para qué fecha. Los profesionales disponibles envían presupuestos. Cuando el cliente elige uno y el profesional confirma, se genera el encargo. Al finalizar el trabajo, el cliente puede calificarlo.

## Integrantes

- Diego Bufalari
- Leonardo Ramos
- Geronimo Carlucci
- Rocco Gironacci

## Cómo ejecutar la API en Windows

Necesitás el SDK de .NET 10, SQL Server Express LocalDB y PowerShell 7.2 o posterior para los comandos de esta guía. Cada integrante puede trabajar con su propia base local. Abrí PowerShell en la carpeta que contiene `ProyectoAppServicios.slnx`.

### 1. Configurar la clave de desarrollo

La primera vez, generá una clave propia para firmar los tokens de acceso y guardala en los secretos locales del proyecto. Si ya configuraste `Jwt:Clave`, continuá con la base de datos.

```powershell
$clave = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
@{ 'Jwt:Clave' = $clave } | ConvertTo-Json -Compress | dotnet user-secrets set --project .\ProyectoAppServicios | Out-Null
Remove-Variable clave
```

Para probar la búsqueda de ubicaciones por dirección o coordenadas, configurá también una clave de Google Maps Platform con la API de geocodificación habilitada. El resto del circuito se puede probar enviando una dirección, latitud y longitud en la propuesta.

```powershell
$maps = Read-Host 'Clave de Google Maps' -MaskInput
@{ 'GoogleMaps:ApiKey' = $maps.Trim() } | ConvertTo-Json -Compress | dotnet user-secrets set --project .\ProyectoAppServicios | Out-Null
Remove-Variable maps
```

### 2. Crear la base y aplicar las migraciones

La conexión predeterminada usa la instancia `(localdb)\MSSQLLocalDB` y la base `AppServiciosDb`. Si usás otra instancia de SQL Server, podés configurar la conexión en el secreto local `ConnectionStrings:DefaultConnection`.

Instalá la herramienta de Entity Framework Core si todavía no la tenés.

```powershell
dotnet tool install --global dotnet-ef --version 10.0.15
```

Creá o actualizá las tablas del proyecto.

```powershell
dotnet ef database update --project .\AppServicios.Infrastructure --startup-project .\ProyectoAppServicios
```

### 3. Iniciar y probar

```powershell
dotnet run --project .\ProyectoAppServicios --launch-profile http
```

Con la API en marcha, abrí el [documento OpenAPI local](http://localhost:5298/openapi/v1.json). Para probar con una base recién creada, abrí [ProyectoAppServicios.http](ProyectoAppServicios/ProyectoAppServicios.http) en Visual Studio y ejecutá estas solicitudes en orden.

1. **Registrar usuario** crea una cuenta de prueba. La respuesta esperada es `201 Created`.
2. **Login** usa el mismo correo y contraseña y devuelve un token con `200 OK`.
3. **Datos del usuario autenticado** envía ese token a `GET /api/auth/yo` y devuelve la cuenta que inició sesión.

## Recorrido de una contratación

| Quién | Solicitud | Resultado |
| --- | --- | --- |
| Cliente | `POST /api/propuestas` | Publica una solicitud con dirección y coordenadas. |
| Profesional disponible | `GET /api/propuestas/activas` | Consulta los trabajos abiertos. |
| Profesional | `POST /api/presupuestos` | Envía una oferta con monto y descripción. |
| Cliente titular | `GET /api/presupuestos/propuesta/{idPropuesta}` | Compara los presupuestos recibidos. |
| Cliente titular | `POST /api/presupuestos/{id}/aceptar` | Selecciona una oferta. |
| Profesional que ofertó | `POST /api/presupuestos/{id}/confirmar` | Confirma el acuerdo y obtiene el identificador del encargo. |
| Profesional asignado | `PUT /api/encargos/{id}/iniciar` y `PUT /api/encargos/{id}/finalizar` | Registra el inicio y el cierre del trabajo. |
| Cliente titular | `POST /api/encargos/{idEncargo}/calificacion` | Califica el trabajo finalizado. |

## Estructura del proyecto

| Proyecto | Responsabilidad |
| --- | --- |
| [ProyectoAppServicios](ProyectoAppServicios/) | Recibe las solicitudes HTTP, aplica la autenticación y autorización, y configura los servicios en `Program.cs`. |
| [AppServicios.Application](AppServicios.Application/) | Coordina casos de uso como publicar una propuesta, seleccionar una oferta o confirmar un encargo. Define los DTO de entrada y salida. |
| [AppServicios.Domain](AppServicios.Domain/) | Contiene las entidades, sus estados y los contratos de los repositorios. La entidad `ProfesionalPropuesta` representa un presupuesto. |
| [AppServicios.Infrastructure](AppServicios.Infrastructure/) | Implementa los repositorios y el acceso a SQL Server con EF Core, la firma de tokens y la integración con Google Maps. |

## Pruebas automáticas

GitHub Actions compila la solución, crea una base LocalDB de prueba, aplica las migraciones y ejecuta solicitudes HTTP contra la API. Entre otros casos, comprueba los permisos, el rechazo de un presupuesto duplicado y que confirmar dos veces devuelva el mismo encargo. Se ejecuta en los pull requests dirigidos a `master` y cuando se actualiza esa rama. [Ver ejecuciones de la CI](https://github.com/LeoRamos98/ProyectoAppServicios/actions/workflows/ci.yml).
