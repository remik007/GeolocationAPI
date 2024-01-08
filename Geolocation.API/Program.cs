using FluentValidation.AspNetCore;
using Geolocation.API.Middleware;
using Geolocation.Api.Data.Context;
using Geolocation.API;
using Geolocation.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Geolocation.Api.DTOs.Validators;
using Geolocation.Api.DTOs;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Settings
var ipStackSettings = new IPStackSettings();
builder.Configuration.GetSection("IPStackApi").Bind(ipStackSettings);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<GeolocationDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IValidator<AddLocationDto>, AddLocationDtoValidator>();
builder.Services.AddScoped<IValidator<LocationDto>, LocationDtoValidator>();
builder.Services.AddScoped<IGeolocationService, GeolocationService>();
builder.Services.AddScoped<IIPStackService, IPStackService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddSingleton(ipStackSettings);
builder.Services.AddHttpClient<IPStackService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }