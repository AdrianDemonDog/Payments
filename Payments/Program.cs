using DemonDog.Contracts.Models;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Payments.Apps.kyc.Interfaces;
using Payments.Apps.kyc.Services;
using Payments.Apps.Mail.Interfaces;
using Payments.Apps.Mail.Services;
using Payments.Apps.Org.Interfaces;
using Payments.Apps.Org.Services;
using Payments.Apps.User.Interfaces;
using Payments.Apps.User.Services;
using Payments.Common.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "PaymentsAPI",
            ValidAudience = "PaymentsAPP",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("k8J5G@3pZr#Yd!2NxLfE$9QvT*Wb^Rm&Cj7AoXhKsU6MqV1Pn")),
            ClockSkew = TimeSpan.Zero
        };
    });

// Configurar Swagger
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payments API",
        Version = "v1",
        Description = "This is the API documentation for PaymentsAPI."
    });

    // Definición de seguridad para Swagger
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // Requerir token en Swagger
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddHttpClient();

// Custom Settings
builder.Configuration.AddJsonFile("CustomSettings.json", optional: true, reloadOnChange: true);

// MongoDB
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient("mongodb+srv://adrianmfer99:passwordprueba@todoapi.2edf1.mongodb.net/"));
builder.Services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("PaymentsDB"));
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// Servicios
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrgService, OrgService>();
builder.Services.AddScoped<IKycService, KycService>();

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<UserRegisteredConsumer>();
    config.AddConsumer<UserLoggedConsumer>();

    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("user-registered-queue", e =>
        {
            e.ConfigureConsumer<UserRegisteredConsumer>(context);
        });

        cfg.ReceiveEndpoint("user-logued-queue", e =>
        {
            e.ConfigureConsumer<UserLoggedConsumer>(context);
        });

        cfg.ReceiveEndpoint("forgot-password-queue", e =>
        {
            e.ConfigureConsumer<ForgotPasswordConsumer>(context);
        });

        cfg.Message<UserRegisteredEvent>(x => x.SetEntityName("user-registered-queue"));
        cfg.Message<UserLoggedEvent>(x => x.SetEntityName("user-logged-queue"));
        cfg.Message<ForgotPasswordEvent>(x => x.SetEntityName("forgot-password-queue"));
    });
});

// Iniciar MassTransit como HostedService
builder.Services.AddMassTransitHostedService();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());


// Configurar el pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
