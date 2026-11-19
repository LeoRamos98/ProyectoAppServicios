using System.Security.Claims;
using AppServicios.Application.Interfaces;
using AppServicios.Application.Services;
using AppServicios.Domain.Interfaces;
using AppServicios.Infrastructure.Repositories;
using AppServicios.Infrastructure.Data;
using AppServicios.Infrastructure.Security;
using AppServicios.Infrastructure.ServiciosExternos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "No se encontró la conexión DefaultConnection.")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Repositorios
builder.Services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProfesionalRepository, ProfesionalRepository>();
builder.Services.AddScoped<IPropuestaRepository, PropuestaRepository>();
builder.Services.AddScoped<IProfesionalPropuestaRepository, ProfesionalPropuestaRepository>();
builder.Services.AddScoped<IEncargoRepository, EncargoRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IMensajeRepository, MensajeRepository>();
builder.Services.AddScoped<ICalificacionRepository, CalificacionRepository>();

// Servicios
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProfesionalService, ProfesionalService>();
builder.Services.AddScoped<IPropuestaService, PropuestaService>();
builder.Services.AddScoped<IProfesionalPropuestaService, ProfesionalPropuestaService>();
builder.Services.AddScoped<IEncargoService, EncargoService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMensajeService, MensajeService>();
builder.Services.AddScoped<ICalificacionService, CalificacionService>();

builder.Services.AddHttpClient(nameof(GoogleMapsService), cliente =>
{
    cliente.BaseAddress = new Uri("https://geocode.googleapis.com/v4/geocode/");
    cliente.Timeout = TimeSpan.FromSeconds(10);
})
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false })
    .AddTypedClient<IUbicacionService>(cliente =>
        new GoogleMapsService(cliente, builder.Configuration["GoogleMaps:ApiKey"]));

// Seguridad
builder.Services.AddSingleton<IHasherContrasenia, HasherContrasenia>();
builder.Services.AddScoped<IAutenticacionService, AutenticacionService>();
builder.Services.AddSingleton<IGeneradorToken, GeneradorTokenJwt>();
builder.Services.AddOptions<JwtOpciones>()
    .BindConfiguration(JwtOpciones.Seccion)
    .ValidateDataAnnotations()
    .Validate(opciones => !string.IsNullOrWhiteSpace(opciones.Clave) && opciones.ClaveEsSuficiente(), "Jwt:Clave debe tener al menos 32 bytes.")
    .ValidateOnStart();

const string esquemaApi = "JwtAplicacion";
var autenticacion = builder.Services.AddAuthentication(esquemaApi)
    .AddJwtBearer(esquemaApi);
builder.Services.AddOptions<JwtBearerOptions>(esquemaApi)
    .Configure<IOptions<JwtOpciones>>((bearer, configuracion) =>
    {
        var jwt = configuracion.Value;
        bearer.MapInboundClaims = false;
        bearer.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Emisor,
            ValidateAudience = true,
            ValidAudience = jwt.Audiencia,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwt.ClaveEnBytes()),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

var politica = new AuthorizationPolicyBuilder(esquemaApi);
if (builder.Environment.IsDevelopment())
{
    // dotnet user-jwts guarda su configuración en el esquema Bearer.
    autenticacion.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);
    politica.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
}
builder.Services.AddAuthorizationBuilder()
    .SetDefaultPolicy(politica.RequireAuthenticatedUser().Build());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();