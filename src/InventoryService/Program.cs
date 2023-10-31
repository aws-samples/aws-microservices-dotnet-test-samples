using InventoryService.BusinessLogic;
using InventoryService.Config;
using InventoryService.DataAccess;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<InventoryDatabaseSettings>(
    builder.Configuration.GetSection(nameof(InventoryDatabaseSettings)));

builder.Services.AddSingleton<IInventoryDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<InventoryDatabaseSettings>>().Value);

builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<InventoryManager>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Enable using Program in Acceptance tests
#pragma warning disable CA1050
public partial class Program
{
}
#pragma warning restore CA1050