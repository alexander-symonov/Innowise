
using Data.Core.Commands;
using Data.Core.Mongo.Commands;
using DataReader;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

var brokersList = Environment.GetEnvironmentVariable("BROKERS_LIST") ?? "localhost:9095";
var incidentType = Environment.GetEnvironmentVariable("INCIDENT_TYPE") ?? "CarIncident";
var topic = "insurance-incidents-" + incidentType;
var groupId = "insurance-incident-group-"+ incidentType;

var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? "mongodb://localhost:27017";
// Configuration.GetConnectionString("MongoDb")

//setup our DI
var serviceProvider = new ServiceCollection()
    //.AddLogging()
    .AddSingleton<IMongoClient>(s => new MongoClient(mongoConnectionString))
    .AddSingleton<IStoreCarIncidentCommand, StoreCarIncidentCommand>()
    .BuildServiceProvider();

var processorFactory = new ProcessorFactory(serviceProvider);
var kafkaConsumer = new DataReader.KafkaConsumer(brokersList, topic, groupId, processorFactory);

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

await kafkaConsumer.ConsumeMessages(cts.Token, incidentType);
