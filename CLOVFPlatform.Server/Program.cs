using CLOVFPlatform.Server.AutoMapper;
using CLOVFPlatform.Server.Models;
using CLOVFPlatform.Server.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Add services to the container.
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<IEmployeeJsonParser, EmployeeJsonParser>();
builder.Services.AddTransient<IEmployeeCsvParser, EmployeeCsvParser>();

builder.Services.AddHttpContextAccessor();

// routing lowercase
builder.Services.AddRouting((options) => { options.LowercaseUrls = true; });
builder.Services.AddControllers().AddNewtonsoftJson((options) => { options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;  });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGenNewtonsoftSupport(); // swagger newtonsoft.json 

builder.Services.AddDbContext<CLOVFContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

