
using Data.Core.Commands;
using Data.Core.Mongo.Commands;
using DataReader;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using DataReader.Settings;

var dbSettings = new InsuranceIncidentsDatabaseSettings();
var kafkaSettings = new KafkaSettings();
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

configuration.Bind(ConfigSections.DatabaseSectionName, dbSettings);
configuration.Bind(ConfigSections.KafkaSectionName, kafkaSettings);

var incidentType = configuration.GetValue<string>("INCIDENT_TYPE") ?? "HealthIncident";
var topic = kafkaSettings.TopicPrefix + incidentType;
var groupId = kafkaSettings.GroupPrefix + incidentType;

//setup our DI
var serviceProvider = new ServiceCollection()
    //.AddLogging()
    .AddSingleton<IMongoClient>(s => new MongoClient(dbSettings.ConnectionString))
    .AddSingleton<IStoreCarIncidentCommand>(x => new StoreCarIncidentCommand(
        x.GetService<IMongoClient>(), 
        dbSettings.DatabaseName, 
        dbSettings.CarIncidentsCollectionName))
    .AddSingleton<IStoreFlatIncidentCommand>(x => new StoreFlatIncidentCommand(
        x.GetService<IMongoClient>(),
        dbSettings.DatabaseName,
        dbSettings.FlatIncidentsCollectionName))
    .AddSingleton<IStoreHealthIncidentCommand>(x => new StoreHealthIncidentCommand(
        x.GetService<IMongoClient>(),
        dbSettings.DatabaseName,
        dbSettings.HealthIncidentsCollectionName))
    .BuildServiceProvider();

var processorFactory = new ProcessorFactory(serviceProvider);
var kafkaConsumer = new DataReader.KafkaConsumer(kafkaSettings.BrokersList, topic, groupId, processorFactory);

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

await kafkaConsumer.ConsumeMessages(cts.Token, incidentType);
