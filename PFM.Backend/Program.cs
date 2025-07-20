using Microsoft.EntityFrameworkCore;
using PFM.Data;
using PFM.Backend.Services;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(">>> Program.cs is running <<<");

// Dodaj DbContext i povezi ga sa PostgreSQL bazom
builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//registrovanje servisa 
builder.Services.AddScoped<TransactionImportService>();

// Dodaj kontrolere i Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Development konfiguracija
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
Console.WriteLine(">>> app.MapControllers() postavljen <<<");
app.Run();
