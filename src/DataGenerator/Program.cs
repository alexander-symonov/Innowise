using DataGenerator;
using System.Text;

var generationProvider = new GenerationProvider();
generationProvider.BrokersList = Environment.GetEnvironmentVariable("KafkaBootstrapServers") ?? "localhost:9095";
generationProvider.TopicNamePrefix = Environment.GetEnvironmentVariable("KafkaTopicNamePrefix") ?? "insurance-incidents-";
generationProvider.ApiUri = Environment.GetEnvironmentVariable("ApiUri") ?? "http://localhost:8085/api/insurance";
var byApiParam = Environment.GetEnvironmentVariable("ByApi") ?? "true";
generationProvider.ByApi = bool.TryParse(byApiParam, out var byApi) && byApi;
generationProvider.DelayMs = int.TryParse(Environment.GetEnvironmentVariable("DelayMs"), out var delay) ? delay : 100;
var botToken = Environment.GetEnvironmentVariable("TelegramBotToken");
using var generationServiceRunningCancelationToken = new CancellationTokenSource();
if (!string.IsNullOrEmpty(botToken))
{
    var telegrammHandlerTask = new TelegramBotHandler(botToken, generationProvider).Run(generationServiceRunningCancelationToken.Token);
}

// Run generation using input params
if (args.Length >= 2)
{
    await generationProvider.Generate(args[0], int.Parse(args[1]), CreateNewState());
}

Console.WriteLine("Enter command. Example: CarIncident 100");
var commandbuilder = new StringBuilder();

// Wait for user input and process commands
await RunLoop();

generationServiceRunningCancelationToken.Cancel();

async Task RunLoop()
{
    while (true)
    {
        if (!generationProvider.Active)
        {
            return;
        }

        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    return;
                case ConsoleKey.Enter:
                    if (!generationProvider.Active)
                    {
                        break;
                    }
                    var commandText = commandbuilder.ToString();
                    var command = commandText.Split(" ");
                    if (command?.Length == 2)
                    {
                        try
                        {
                            await generationProvider.Generate(command[0], int.Parse(command[1]), CreateNewState());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid command :" + commandText);
                    }
                    commandbuilder.Clear();
                    break;
                default:
                    commandbuilder.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                    break;
            }
        }
    }
}

StateMediator CreateNewState()
{
    var latestGenerationState = new CancellationTokenSource();
    return new StateMediator(latestGenerationState.Token);
}