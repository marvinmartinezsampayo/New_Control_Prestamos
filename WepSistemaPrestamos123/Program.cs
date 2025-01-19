using Datos.Contexto;
using Datos.Contratos.Login;
using Datos.Contratos.Solicitud;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Negocio.Gestion;
using Negocio.Implementacion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login";
        options.AccessDeniedPath = "/Login/CerrarSesion";
        options.LogoutPath = "/Login/CerrarSesion";
        options.Cookie.Name = "Prestamo";
        options.Cookie.HttpOnly = true;
        //vencimiento
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
        options.Cookie.MaxAge = options.ExpireTimeSpan;
        options.SlidingExpiration = true;
        // ReturnUrlParameter requires 
        options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    });

//cadena conexión

builder.Services.AddDbContext<ContextoGeneral>(options =>
{    //options.UseMySql("Server=bruw5ccc7yrcevwil4di-mysql.services.clever-cloud.com;Database=bruw5ccc7yrcevwil4di;User=uu0kkxfc1wgirqb9;Password=H3VfcvSKWnOvwZxfobt5;Port=3306;",
    //        new MySqlServerVersion(new Version(8, 0, 31)));
    options.UseMySQL(builder.Configuration.GetConnectionString("strConexionMySql"));
});

builder.Services.AddDbContext<ContextoLocal>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("strConexionMySqlLocal"));
});

// Seguridad
builder.Services.AddScoped<IGestionUsuario, Gestion_Login>();
builder.Services.AddScoped<ILugaresGeograficos, Gestion_Lugares_Geograficos>();
builder.Services.AddScoped<IGenerar_Codigo, Gestion_Codigo_Acceso>();


builder.Services.AddAuthorization();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "area",
    pattern: "{area:exists}/{controller=Ingreso}/{action=Registro}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//      name: "areas",
//      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
//    );
//});


app.Run();
