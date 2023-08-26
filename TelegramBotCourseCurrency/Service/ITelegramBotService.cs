using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotCourseCurrency.Service
{
    public interface ITelegramBotService
    {
        Task<bool> CalculateCurrencyCourse(Update upd);

        Task<TelegramBotClient> GetClient();
    }
}
