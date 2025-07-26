using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PFM.Backend.Database.Entities;
using PFM.Backend.Middleware;
using PFM.Backend.Services;
using PFM.Data;
using PFM.Backend.Models.AutomaticCategorization;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(">>> Program.cs is running <<<");

//povezivanje sa bazom
builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Configuration.AddJsonFile("RulesConfig.json", optional: false, reloadOnChange: false);


//registrovanje servisa 
builder.Services.AddScoped<TransactionImportService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();


builder.Services.AddScoped<ICategoryImportService, CategoryImportService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PFM API", Version = "v1" });
    c.OperationFilter<AddCsvRequestBodyFilter>();
});
// options =>
// {
//     options.MapType<Kind>(() => new OpenApiSchema
//     {
//         Type = "string",
//         Enum = Enum.GetNames(typeof(Kind))
//             .Select(n => new OpenApiString(n))
//             .Cast<IOpenApiAny>()
//             .ToList()
//     });
// });

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new KebabCaseNamingPolicy();
    });

    builder.Services.Configure<RulesConfig>(
        builder.Configuration
    );

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

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
