using MongoDB.Driver;
using Payments.Apps.Mail.Interfaces;
using Payments.Apps.Mail.Services;
using Payments.Apps.Org.Interfaces;
using Payments.Apps.Org.Services;
using Payments.Apps.User.Interfaces;
using Payments.Apps.User.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient("mongodb+srv://adrianmfer99:passwordprueba@todoapi.2edf1.mongodb.net/"));
builder.Services.AddScoped<IMongoDatabase>(static sp => sp.GetRequiredService<IMongoClient>().GetDatabase("PaymentsDB"));
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// Servicios
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrgService, OrgService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
