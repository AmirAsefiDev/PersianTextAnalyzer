using Microsoft.OpenApi.Models;
using PersianTextAnalyzer.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(s =>
{
    s.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PersianTextAnalyzer.Api.xml"), true);
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PersianTextAnalyzer.Api",
        Version = "v1"
    });
});

builder.Services.AddHttpClient<SaplingController>(client => { client.Timeout = TimeSpan.FromSeconds(10); });

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI
(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PersianTextAnalyzer API");
    c.RoutePrefix = "swagger"; //Show Swagger in route domain
});
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();