using System.Net;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Xml;
using System.Text;
using System.Globalization;

namespace TelegramBotCourseCurrency.Service
{
    public class TelegramBotService : ITelegramBotService
    {
        private TelegramBotClient _client;

        public TelegramBotService()
        {

        }

        public async Task<bool> CalculateCurrencyCourse(Update upd)
        {
            if (upd?.Message?.Chat == null && upd?.CallbackQuery == null) return false;

            if (upd.Message != null && upd.Message.Text.Contains("start"))
                await ExecuteStart(upd);

            return true;
        }

        public async Task<TelegramBotClient> GetClient()
        {
            if (_client != null) return _client;

            _client = new TelegramBotClient("6526445897:AAHzmNoJITjSwtd-62Lg-6exZ7mrlnGcNVg");
            return _client;
        }

        private async Task<bool> ExecuteStart(Update upd)
        {
            WebClient client = new WebClient();
            string text = client.DownloadString("https://www.nationalbank.kz/rss/rates_all.xml");

            if (string.IsNullOrEmpty(text))
            {
                await _client.SendTextMessageAsync(upd.Message.Chat.Id, "При запросе к курсу валют что-то пошло не так.\n\nОбратитесь к менеджеру.\n\n@silklink_cargo");
                return false;
            }

            
            try
            {
                var listNodes = new List<XmlNode>();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(text);
                foreach (XmlNode nodes in doc.DocumentElement.ChildNodes)
                {
                    foreach (XmlNode node in nodes)
                    {
                        if (node.Name.Contains("item"))
                            listNodes.Add(node);
                    }
                }


                StringBuilder strBuilder = new StringBuilder();
                strBuilder.AppendLine("Покупка - Продажа\n");
                foreach (XmlNode node in listNodes)
                {
                    var nd = node["title"];
                    if (node["title"].InnerText.Contains("USD"))
                    {
                        if (double.TryParse(node["description"].InnerText, NumberStyles.Any, CultureInfo.InvariantCulture ,out double usdCourse))
                        {
                            var usdCourseeBuy = usdCourse - 10;
                            var usdCourseSell = usdCourse + 10;
                            strBuilder.AppendLine($"🇺🇸 USD {usdCourseeBuy} - {usdCourseSell}\n");
                        }
                    }
                    else if (node["title"].InnerText.Contains("CNY"))
                    {
                        if (double.TryParse(node["description"].InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out double cnyCourse))
                        {
                            var cnyCourseBuy = cnyCourse - 1;
                            var cnyCourseSell = cnyCourse + 4;
                            strBuilder.AppendLine($"🇨🇳 CNY {cnyCourseBuy} - {cnyCourseSell}\n");
                        }
                    }
                    else continue; 
                }

                strBuilder.AppendLine("Чтобы купить или продать Валюту обратитесь к менеджеру\n\n@silklink_cargo");
                await _client.SendTextMessageAsync(upd.Message.Chat.Id, strBuilder.ToString());
                return true;
            }
            catch (Exception)
            {
                await _client.SendTextMessageAsync(upd.Message.Chat.Id, "При запросе к курсу валют что-то пошло не так.\n\nОбратитесь к менеджеру.\n\n@silklink_cargo");
                return false;
            }
        }
    }
}
