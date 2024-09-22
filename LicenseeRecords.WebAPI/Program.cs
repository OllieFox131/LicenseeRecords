using FluentValidation;
using LicenseeRecords.Models;
using LicenseeRecords.WebAPI.Data;
using LicenseeRecords.WebAPI.Repositories.Interfaces;
using LicenseeRecords.WebAPI.Repositories.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDataManager, DataManager>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IValidator<Account>, AccountValidator>();
builder.Services.AddScoped<IValidator<ProductLicence>, ProductLicenceValidator>();
builder.Services.AddScoped<IValidator<Product>, ProductValidator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
