using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DataGenerator
{
    internal class TelegramBotHandler : IUpdateHandler
    {
        private GenerationProvider _generationProvider;
        ITelegramBotClient _botClient;
        CancellationTokenSource _latestGeneration = new CancellationTokenSource();

        public TelegramBotHandler(string botToken, GenerationProvider generationProvider)
        {
            _generationProvider = generationProvider;
            _botClient = new TelegramBotClient(botToken);
        }

        public async Task Run(CancellationToken token)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                }
            };

            _botClient.StartReceiving(this, receiverOptions, token);

            var me = await _botClient.GetMe();
            Console.WriteLine($"{me.FirstName} running!");
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update?.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                var chatId = message?.Chat.Id ?? 0;

                switch (message?.Text)
                {
                    case "/start":
                        await botClient.SendMessage(
                            chatId,
                            $"{GetMenuInfo()}\n\n{GetSettingsInfo()}",
                            replyMarkup: GetMenu(),
                            parseMode: ParseMode.Html
                        );
                        break;

                    case "/help":
                        await botClient.SendMessage(
                            chatId,
                           GetMenuInfo(),
                            replyMarkup: GetMenu()
                        );
                        break;

                    //case "/stop":
                    //    _latestGeneration?.Cancel();

                    //    await botClient.SendMessage(
                    //        chatId,
                    //        "Services stopped"
                    //    );
                    //    var stopingTask = Task.Delay(100).ContinueWith((t) => _generationProvider.Active = false);

                    //    break;

                    case "settings":
                        await botClient.SendMessage(
                            chatId,
                            "Settings",
                            replyMarkup: GetSettingsMenu()
                        );
                        break;

                    case "/generate":
                        await botClient.SendMessage(
                            chatId,
                            "Select event type",
                            replyMarkup: GetGenerationMenu()
                        );
                        break;
                }
            }
            else if (update?.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                var callbackQuery = update.CallbackQuery;
                var chatId = callbackQuery?.Message?.Chat.Id ?? 0;

                switch (callbackQuery.Data)
                {
                    case string data when data.StartsWith("generate"):
                        await botClient.SendMessage(
                            chatId,
                            "Select event type",
                            replyMarkup: GetGenerationMenu()
                        );
                        break;

                    case string data when (data.StartsWith("CarIncident")
                                            || data.StartsWith("FlatIncident")
                                            || data.StartsWith("HealthIncident")):
                        await botClient.SendMessage(
                            chatId,
                            "Select count",
                            replyMarkup: GetCountMenu(callbackQuery.Data)
                        );
                        break;

                    case string data when data.StartsWith("count_"):
                        GenerateEvents(botClient, callbackQuery, chatId);
                        break;

                    case "stop_gen":
                        _latestGeneration?.Cancel();
                        await botClient.SendMessage(
                            chatId,
                            "Generation cancelled."
                        );
                        break;

                    case "settings":
                        await botClient.SendMessage(
                            chatId,
                            "Settings",
                            replyMarkup: GetSettingsMenu()
                        );
                        break;

                    case "useapi":
                        _generationProvider.ByApi = true;
                        await botClient.SendMessage(
                            chatId,
                            "Using API",
                            replyMarkup: new ReplyKeyboardRemove()
                        );
                        await botClient.SendMessage(
                            chatId,
                            GetSettingsInfo(),
                            parseMode: ParseMode.Html
                        );
                        await botClient.DeleteMessage(chatId, callbackQuery.Message.MessageId);
                        break;

                    case "usekafka":
                        _generationProvider.ByApi = false;
                        await botClient.SendMessage(
                            chatId,
                            "Using Kafka",
                            replyMarkup: new ReplyKeyboardRemove()
                        );
                        await botClient.SendMessage(
                            chatId,
                            GetSettingsInfo(),
                            parseMode: ParseMode.Html
                        );
                        await botClient.DeleteMessage(chatId, callbackQuery.Message.MessageId);
                        break;

                    case "setdelay":
                        await botClient.SendMessage(
                            chatId,
                            "Select delay",
                            replyMarkup: GetDeleayMenu()
                        );
                        break;

                    case string data when data.StartsWith("delay_"):
                        var delayParts = callbackQuery.Data.Split('_');
                        var delay = int.Parse(delayParts[1]);
                        _generationProvider.DelayMs = delay;
                        await botClient.SendMessage(
                            chatId,
                            $"Delay set to {delay} ms",
                            replyMarkup: new ReplyKeyboardRemove()
                        );
                        await botClient.DeleteMessage(chatId, callbackQuery.Message.MessageId);
                        await botClient.SendMessage(
                            chatId,
                            GetSettingsInfo(),
                            parseMode: ParseMode.Html
                        );
                        break;                    

                    default:
                        await botClient.SendMessage(
                            chatId,
                            "Unknown command " + callbackQuery.Data
                        );
                        break;
                }
            }
        }

        private void GenerateEvents(ITelegramBotClient botClient, CallbackQuery callbackQuery, long chatId)
        {
            var parts = callbackQuery.Data.Split('_');
            var count = int.Parse(parts[2]);
            var type = parts[1];
            var cancelationTockenSource = new CancellationTokenSource();
            var state = new StateMediator(cancelationTockenSource.Token);
            var deleteMessageTask = botClient.DeleteMessage(chatId, callbackQuery.Message.MessageId);
            var startMessageTask = botClient.SendMessage(chatId, $"Start generating {count} {type} events.");
            var stateMessageTask = botClient.SendMessage(chatId, $"Generating {count} {type} events...")
                .ContinueWith(async (t) =>
                {
                    var messageId = t.Result.MessageId;
                    do
                    {
                        await Task.Delay(1000);
                        await botClient.EditMessageText(
                            chatId,
                            messageId,
                            $"Generating {count} {type} events...\n" +
                            $"Success: {state.SucsessCounter}\n" +
                            $"Failed: {state.FailedCounter}",
                            replyMarkup: new InlineKeyboardMarkup(new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Stop", "stop_gen")
                            })
                        );
                    } while (!state.Token.IsCancellationRequested);
                    await botClient.DeleteMessage(chatId, messageId);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

            var generationTask = Task.Run(async() =>
            {
                try
                {
                    _latestGeneration = cancelationTockenSource;
                    await _generationProvider.Generate(type, count, state);
                    await botClient.SendMessage(
                        chatId,
                        $"Generating {count} {type} events done.\nSuccess: {state.SucsessCounter}\nFailed: {state.FailedCounter}"
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    await botClient.SendMessage(
                        chatId,
                        "Error during generation: " + ex.Message
                    );
                }
                finally
                {
                    cancelationTockenSource.Cancel();
                }
            });            
        }

        private string GetMenuInfo()
        {
            return "Allowed commands:\n/help\n/generate\n/settings\n/stop\n";
        }

        private string GetSettingsInfo()
        {
            var sendVia = _generationProvider.ByApi ? "API" : "Kafka";
            return  "          <b>Current Settings:</b>\n" +
                    $"Brokers:      <i>{_generationProvider.BrokersList}</i>\n" +
                    $"ApiUri:       <i>{_generationProvider.ApiUri}</i>\n" +
                    $"Send via:     <i>{sendVia}</i>\n" +
                    $"Delay (ms):   <i>{_generationProvider.DelayMs}</i>\n" +
                    $"Topic prefix: <i>{_generationProvider.TopicNamePrefix}</i>";
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error: {exception.Message}");
            return Task.CompletedTask;
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private InlineKeyboardMarkup GetMenu()
        {
            var keyboard = new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Events generation", "generate"),
                    InlineKeyboardButton.WithCallbackData("Settings", "settings")
                }
            };

            return new InlineKeyboardMarkup(keyboard);
        }

        private InlineKeyboardMarkup GetSettingsMenu()
        {
            var keyboard = new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Change delay", "setdelay"),
                }
            };
            keyboard[0] = _generationProvider.ByApi
                ? keyboard[0].Append(InlineKeyboardButton.WithCallbackData("Use Kafka", "usekafka")).ToArray()
                : keyboard[0].Append(InlineKeyboardButton.WithCallbackData("Use API", "useapi")).ToArray();

            return new InlineKeyboardMarkup(keyboard);
        }

        private InlineKeyboardMarkup GetGenerationMenu()
        {
            var keyboard = new[]
            {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Car Incident", "CarIncident"),
                        InlineKeyboardButton.WithCallbackData("Flat Incident", "FlatIncident"),
                        InlineKeyboardButton.WithCallbackData("Health Incident", "HealthIncident")
                    }
                };
            return new InlineKeyboardMarkup(keyboard);
        }

        private InlineKeyboardMarkup GetCountMenu(string type)
        {
            var keyboard = new[]
            {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("10", $"count_{type}_10"),
                        InlineKeyboardButton.WithCallbackData("100", $"count_{type}_100"),
                        InlineKeyboardButton.WithCallbackData("1K", $"count_{type}_1000"),
                        InlineKeyboardButton.WithCallbackData("10K", $"count_{type}_10000"),
                        InlineKeyboardButton.WithCallbackData("100K", $"count_{type}_100000"),
                    }
                };
            return new InlineKeyboardMarkup(keyboard);
        }

        private InlineKeyboardMarkup GetDeleayMenu()
        {
            var keyboard = new[]
            {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("None", $"delay_0"),
                        InlineKeyboardButton.WithCallbackData("1", $"delay_1"),
                        InlineKeyboardButton.WithCallbackData("10", $"delay_10"),
                        InlineKeyboardButton.WithCallbackData("100", $"delay_100"),
                        InlineKeyboardButton.WithCallbackData("1K", $"delay_1000"),
                    }
                };
            return new InlineKeyboardMarkup(keyboard);
        }
    }
}
