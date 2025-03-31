using DataGenerator;

var deleayMs = 100;
var brokersList = Environment.GetEnvironmentVariable("BROKERS_LIST") ?? "localhost:9095";
var eventGenerator = new EventGenerator(brokersList);
if(args.Length >= 2)
{
    Run(args[0], int.Parse(args[1]));
}

do
{
    Console.WriteLine("Enter command. Example: CarIncident 100");
    var command = Console.ReadLine().Split(" ");
    if (command.Length == 2) {
        Run(command[0], int.Parse(command[1])); 
    }
    else { 
        Console.WriteLine("Invalid command"); 
    }

} while (true);



async Task Run(string type, int count)
{
    try
    {
        await eventGenerator.Generate(type, count, deleayMs);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}