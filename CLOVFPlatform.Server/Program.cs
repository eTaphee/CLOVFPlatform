using CLOVFPlatform.Server.AutoMapper;
using CLOVFPlatform.Server.Models;
using CLOVFPlatform.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;

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
builder.Services.AddSwaggerGen((options) =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "CLO VF Platform API",
        Description = "An ASP.NET Core Web API for clovf employee",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
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

