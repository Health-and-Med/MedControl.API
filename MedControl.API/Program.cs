﻿using MedControl.Application.Services;
using MedControl.Domain.Interfaces;
using MedControl.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configurar leitura de arquivos de configuração
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory()) // Define o diretório base
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Carrega appsettings.json
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true) // Carrega appsettings.Development.json em ambiente de dev
    .AddEnvironmentVariables(); // Permite sobrescrever via variáveis de ambiente


// 🔹 Configuração da string de conexão com PostgreSQL
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


builder.Services.AddScoped<IMedicoRepository, MedicoRepository>();
builder.Services.AddScoped<IUsuariosMedicosRepository, UsuariosMedicosRepository>();
builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<IUsuariosMedicosService, UsuariosMedicosService>();

// 🔹 Conexão com o banco de dados PostgreSQL
builder.Services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(connectionString));

// 🔹 Configuração do JWT (Deve ser igual à configuração da AuthMed API)
var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtConfig["Secret"]!);


builder.Services.AddAuthentication("Bearer") // 🔹 Nome do esquema deve ser igual ao do Ocelot
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"]
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 🔹 Configuração do Swagger para suportar JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MedControl.API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Digite 'Bearer {seu_token_jwt}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
    c.OperationFilter<AuthenticationRequirementsOperationFilter>();
});


var app = builder.Build();

// 🔹 Inicializa banco de dados (somente se necessário)
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    dbInitializer.Initialize();
}

// 🔹 Configuração do Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MedControl.API V1");
        c.DisplayRequestDuration(); // Exibe tempo de resposta nas requisições
    });
}

// 🔹 Middleware de autenticação e autorização (na ordem correta)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

// 🔹 Configuração do Swagger para proteger os endpoints autenticados
public class AuthenticationRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Security == null)
            operation.Security = new List<OpenApiSecurityRequirement>();

        var scheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        };
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [scheme] = new List<string>()
        });
    }
}


