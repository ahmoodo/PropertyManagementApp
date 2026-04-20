using PropertyManagementApp.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<InMemoryDataStore>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// seed some fake data on startup
var store = app.Services.GetRequiredService<InMemoryDataStore>();
store.Seed();

app.UseCors();
app.MapControllers();

app.Run();
