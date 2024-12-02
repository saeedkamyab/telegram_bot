using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace telegram_bot.Servises
{
    public class TelegramBotService
    {
        private readonly ITelegramBotClient _botClient;

        public TelegramBotService(IConfiguration configuration)
        {
            var token = configuration["BotConfiguration:BotToken"];
            _botClient = new TelegramBotClient(token); 
        }
        public async Task StartBotAsync(CancellationToken cancellationToken = default)
        {
            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"Bot {me.Username} is running...");

            _botClient.StartReceiving(
                   HandleUpdateAsync,
                 HandleErrorAsync,
                cancellationToken: cancellationToken
            );
        }
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;

                if (messageText.StartsWith("/start"))
                {
                    await botClient.SendTextMessageAsync(chatId, "به ربات خوش آمدید!", cancellationToken: cancellationToken);
                }
                else if (messageText.StartsWith("/register"))
                {
                    // مدیریت ثبت‌نام کاربران
                    await botClient.SendTextMessageAsync(chatId, "لطفاً اطلاعات ثبت‌نام خود را ارسال کنید.", cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, "دستور ناشناخته است.", cancellationToken: cancellationToken);
                }
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error: {exception.Message}");
            return Task.CompletedTask;
        }
    
    public async Task SendMessage(string chatId, string message)
        {
            try
            {
                await _botClient.SendMessage(
                    chatId: chatId,
                    text: message
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
