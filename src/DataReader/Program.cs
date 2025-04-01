
var brokersList = Environment.GetEnvironmentVariable("BROKERS_LIST") ?? "localhost:9095";
var incidentType = Environment.GetEnvironmentVariable("INCIDENT_TYPE") ?? "CarIncident";
var topic = "insurance-incidents-" + incidentType;
var groupId = "insurance-incident-group-"+ incidentType;

var kafkaConsumer = new DataReader.KafkaConsumer(brokersList, topic, groupId);

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

await kafkaConsumer.ConsumeMessages(cts.Token, incidentType);
