using BookManager.API.Extensions;
using BookManager.API.Middleware;
using BookManager.Application;
using BookManager.Infrastructure;
using BookManager.Infrastructure.Data;
using FastEndpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

DependencyInjection.ApplyMigrations(app.Services);

app.MapScalarApiReference();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();


await SeedData.SeedAsync(app.Services);

app.Run();

