using ClinicaAPI.Interface;
using ClinicaAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Resend;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURACIÓN
// ============================================

// Leer appsettings.json + variables de entorno
builder.Configuration.AddJsonFile(
    "appsettings.json",
    optional: false,
    reloadOnChange: true
);

builder.Configuration.AddEnvironmentVariables();

// ============================================
// JWT
// ============================================

var jwtSettings = builder.Configuration.GetSection("Jwt");

var keyString = jwtSettings["Key"]
    ?? throw new Exception("JWT Key no configurada");

var key = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(keyString)
);

// ============================================
// CORS
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
// SERVICIOS BASE
// ============================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// ============================================
// RESEND
// ============================================

builder.Services.AddOptions();

var resendApiKey = builder.Configuration["Resend__ApiKey"];

Console.WriteLine($"RESEND => {resendApiKey}");

if (string.IsNullOrWhiteSpace(resendApiKey))
{
    throw new Exception("Resend ApiKey no configurada");
}

builder.Services.Configure<ResendClientOptions>(options =>
{
    options.ApiToken = resendApiKey;
});

builder.Services.AddHttpClient();

builder.Services.AddTransient<IResend, ResendClient>();

builder.Services.AddScoped<IEmailService, EmailService>();

// ============================================
// WHATSAPP
// ============================================

builder.Services.AddHttpClient<WhatsAppService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<NotificacionesService>();

builder.Services.AddHostedService<WhatsAppSchedulerService>();

// ============================================
// SWAGGER
// ============================================

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
        { securityScheme, Array.Empty<string>() }
    });
});

// ============================================
// AUTH JWT
// ============================================

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
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

builder.Services.AddAuthorization();

builder.Services.AddSingleton<AuthService>();

// ============================================
// APP
// ============================================

var app = builder.Build();

app.MapGet("/", () => "Clinica API funcionando");

// ============================================
// MIDDLEWARES
// ============================================

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "Clinica API V1"
    );
});

app.UseCors("_myAllowOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();