using Amazon.DynamoDBv2;
using Amazon.SQS;
using Microsoft.Extensions.Options;
using ShoppingCartService.BusinessLogic;
using ShoppingCartService.Config;
using ShoppingCartService.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dynamoDbConfig = builder.Configuration.GetSection("DynamoDb");
var runLocalDynamoDb = dynamoDbConfig.GetValue<bool>("LocalMode");

if (runLocalDynamoDb)
    builder.Services.AddSingleton<IAmazonDynamoDB>(_ =>
    {
        var clientConfig = new AmazonDynamoDBConfig { ServiceURL = dynamoDbConfig.GetValue<string>("LocalServiceUrl") };
        return new AmazonDynamoDBClient(clientConfig);
    });
else
    builder.Services.AddAWSService<IAmazonDynamoDB>();

builder.Services.AddAWSService<IAmazonSQS>();

builder.Services.Configure<ExternalServicesSettings>(
    builder.Configuration.GetSection(nameof(ExternalServicesSettings)));

builder.Services.AddSingleton<IExternalServicesSettings>(sp =>
    sp.GetRequiredService<IOptions<ExternalServicesSettings>>().Value);

builder.Services.AddSingleton<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddSingleton<IOrderServiceNotifications, OrderServiceNotifications>();
builder.Services.AddSingleton<IInventoryRepository, InventoryRepository>();
builder.Services.AddSingleton<ShoppingCartManager>();
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