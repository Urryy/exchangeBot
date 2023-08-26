using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotCourseCurrency.Service;

namespace TelegramBotCourseCurrency
{
    public class TelegramBot
    {
        private readonly IConfiguration _configuration;
        private readonly ITelegramBotService _telegramBotService;
        private TelegramBotClient _client;


        public TelegramBot(IConfiguration configuration, ITelegramBotService telegramBotService)
        {
            _configuration = configuration;
            _telegramBotService = telegramBotService;
        }

        public async Task<TelegramBotClient> StartRecieveBot()
        {
            try
            {
                if (_client != null) 
                {
                    await _client.ReceiveAsync(Update, Error);
                    return _client; 
                }
                _client = await _telegramBotService.GetClient();

                await _client.ReceiveAsync(Update, Error);
                return _client;
            }
            catch (Exception ex)
            {
                await _client.CloseAsync();
                await StartRecieveBot();
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private async Task Error(ITelegramBotClient client, Exception exc, CancellationToken token)
        {
            //await client.DeleteWebhookAsync();
            Console.WriteLine(exc.Message);
            Debug.WriteLine(exc.Message);
            if (_client.Timeout.Minutes > 1)
            {
                await _client.CloseAsync();
                await StartRecieveBot();
            }
            return;
        }

        private async Task Update(ITelegramBotClient client, Update upd, CancellationToken token)
        {
            try
            {
                if (_client.Timeout.Minutes > 1)
                {
                    await _client.CloseAsync();
                    await StartRecieveBot();
                }


                await _telegramBotService.CalculateCurrencyCourse(upd);
                return;
            }
            catch (Exception)
            {
                await _client.CloseAsync();
                await StartRecieveBot();
            }
            
        }
    }
}
