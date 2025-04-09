using DataGenerator;

var deleayMs = 100;
var brokersList = Environment.GetEnvironmentVariable("Kafka__BootstrapServers") ?? "localhost:9095";
var apiUri = Environment.GetEnvironmentVariable("ApiUri") ?? "http://localhost:8085/api/insurance";
var byApiParam = Environment.GetEnvironmentVariable("ByApi");
var byApi = true;
if(!string.IsNullOrEmpty(byApiParam))
{
    byApi = bool.Parse(byApiParam);
}
var eventGenerator = new EventGenerator();
if(args.Length >= 2)
{
    await Run(args[0], int.Parse(args[1]));
}

do
{
    Console.WriteLine("Enter command. Example: CarIncident 100");
    var commandText = Console.ReadLine();
    var command = commandText?.Split(" ");
    if (command?.Length == 2) {
        await Run(command[0], int.Parse(command[1])); 
    }
    else { 
        Console.WriteLine("Invalid command :" + commandText); 
    }
} while (true);


async Task Run(string type, int count)
{
    try
    {
        if(byApi)
        {
            await eventGenerator.SendMessageToApi(apiUri, type, count, deleayMs);
        }
        else
        {
            await eventGenerator.GenerateForKafka(brokersList, type, count, deleayMs);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}