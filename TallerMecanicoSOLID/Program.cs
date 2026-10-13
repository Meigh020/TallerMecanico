using TallerMecanicoSOLID.Configuracion;
using TallerMecanicoSOLID.Interfaces;
using TallerMecanicoSOLID.Models;
using TallerMecanicoSOLID.Repositories;

var builder = WebApplication.CreateBuilder(args);


// Cadena de conexión

string connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión DefaultConnection.");


// Singleton

ConfiguracionBD configuracionBD =
    ConfiguracionBD.ObtenerInstancia(connectionString);

builder.Services.AddSingleton(configuracionBD);


// Repositorios

builder.Services.AddScoped<IRepositorio<Cliente>, RepositorioCliente>();

builder.Services.AddScoped<IRepositorio<Vehiculo>, RepositorioVehiculo>();

builder.Services.AddScoped<IRepositorio<Servicio>, RepositorioServicio>();


// Data Protection

builder.Services.AddDataProtection();


// Razor Pages

builder.Services.AddRazorPages();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");

    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();