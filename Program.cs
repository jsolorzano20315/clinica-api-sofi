using ClinicaAPI.Interface;
using ClinicaAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Resend;

var builder = WebApplication.CreateBuilder(args);

// JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var keyString = jwtSettings["Key"] ?? throw new Exception("JWT Key no configurada");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

// 🔥 IMPORTANTE PARA RENDER
 var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});

// ============================================
// Configurar CORS
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("_myAllowOrigins", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:5174",
            "https://clinica-app-sofi.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// ============================================
// Servicios
// ============================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// 🔥 RESEND CONFIG
builder.Services.AddOptions();

Console.WriteLine("RESEND KEY:");
Console.WriteLine(key);
Console.WriteLine("NULL? " + (key == null));

builder.Services.Configure<ResendClientOptions>(options =>
{
    options.ApiToken = builder.Configuration["Resend:ApiKey"]?.Trim();
});

// 🔥 ESTO es lo que inyecta HttpClient correctamente
builder.Services.AddHttpClient<ResendClient>();

// 🔥 Tu abstracción
builder.Services.AddScoped<IResend, ResendClient>();

// 🔥 Tu servicio
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHttpClient<WhatsAppService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<NotificacionesService>();

builder.Services.AddHostedService<WhatsAppSchedulerService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clinica API",
        Version = "v1"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

// ============================================
// JWT
// ============================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = key
    };
});

builder.Services.AddSingleton<AuthService>();

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapGet("/", () => "Clinica API funcionando");

// ============================================
// Middlewares
// ============================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinica API V1");
    });
}

app.UseCors("_myAllowOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();