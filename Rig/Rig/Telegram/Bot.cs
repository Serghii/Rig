using System.Threading.Tasks;
using Telegram.Bot;

namespace Rig.Telegram.Model
{
    public static class Bot
    {
        private static TelegramBotClient client;

        public static async Task<TelegramBotClient> Get(string botId = null)
        {
            if (client != null)
            {
                return client;
            }
            if (string.IsNullOrEmpty(botId))
            {
                return null;
            }
            client = new TelegramBotClient(botId);
            client.StartReceiving();

            return client;
        }

        public static TelegramBotClient Client => Get().Result;

    }
}
