using Confluent.Kafka;
using DataAPI.Modules.Insurance;

var insuranceConfigSectionName = "InsuranceModule";
var insuranceConfiguration = new Configuration();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.Bind(insuranceConfigSectionName, insuranceConfiguration);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Title = "Data API",
        Version = "v1"
    });
});var section = builder.Configuration.GetSection(insuranceConfigSectionName);
RegisterKafka(builder);
builder.Services.RegisterInsuranceModule();
builder.Services.Configure<DataAPI.Modules.Insurance.Configuration>(
    builder.Configuration.GetSection(insuranceConfigSectionName));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.RegisterInsuranceEndpoints(insuranceConfiguration.ApiPrefix);

app.Run();

static void RegisterKafka(WebApplicationBuilder builder)
{
    var kafkaConfiguration = new DataAPI.Configuration.KafkaConfiguration();
    builder.Configuration.Bind("Kafka", kafkaConfiguration);
    var producerConfig = new ProducerConfig
    {
        BootstrapServers = kafkaConfiguration.BootstrapServers,
        ClientId = kafkaConfiguration.ClientId,
        MessageTimeoutMs = kafkaConfiguration.MessageTimeoutMs
    };
    builder.Services.AddSingleton(new ProducerBuilder<string, byte[]>(producerConfig).Build());
}