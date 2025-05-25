using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using ReportsAPI.Modules.Insurance;
using ReportsAPI.Modules.User;
using ReportsAPI.Settings;

var corsPolicyName = "CorsPolicy";
MongoDbSettings mongoDbSettings = new MongoDbSettings();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.Bind("MongoDbSettings", mongoDbSettings);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = MongoClientSettings.FromConnectionString(mongoDbSettings.ConnectionString);
    return new MongoClient(settings);
});
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDbSettings.DatabaseName);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName, policy =>
    {
        policy.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin();
    });
});

builder.Services.RegisterInsuranceModule();
builder.Services.RegisterUserModule(mongoDbSettings);

ConventionRegistry.Register(
    "IgnoreExtraElements",
    new ConventionPack { new IgnoreExtraElementsConvention(true), new CamelCaseElementNameConvention() },
    t => true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.RegisterUserEndpoints("/api/user");
app.RegisterInsuranceEndpoints("/api/insurance");

app.Run();
