using ClinicaAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var keyString = jwtSettings["Key"] ?? throw new Exception("JWT Key no configurada");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

// ============================================
// Configurar CORS (CORREGIDO)
// ============================================
var MyAllowOrigins = "_myAllowOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowOrigins, policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:5174",
            "https://clinica-api-sofi.onrender.com/"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
        //.AllowCredentials();
    });
});

// ============================================
// Servicios
// ============================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Clinica API", Version = "v1" });

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
// Autenticación JWT
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

// 👇 IMPORTANTE: CORS antes de auth
//app.UseRouting();

app.UseCors(MyAllowOrigins);
//////app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 🔥 IMPORTANTE PARA CLOUD (Render, Azure, etc.)
//var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
//app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();