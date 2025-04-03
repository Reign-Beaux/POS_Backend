using Web.API;
using Application;
using Infrastructure;
using Web.API.Extensions;

var cors = "Cors";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddAPI(builder.Configuration)
    .AddApplication()
    .AddInfraestructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
      name: cors,
      builder =>
      {
          builder
              .WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();
app.UseCors(cors);
app.UseAuthorization();
app.MapControllers();
app.Run();
