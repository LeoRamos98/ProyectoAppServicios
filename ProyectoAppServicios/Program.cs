using AppServicios.Application.Interfaces;
using AppServicios.Application.Services;
using AppServicios.Domain.Interfaces;
using AppServicios.Infrastructure.Repositories;
using AppServicios.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "No se encontró la conexión DefaultConnection.")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Repositorios
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();