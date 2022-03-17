using System.IO;
using Newtonsoft.Json;

namespace Bones
{
    static class Config
    {
        public static readonly BotConfig bot;


        static Config()
        {
            if (!Directory.Exists("Resources"))
                Directory.CreateDirectory("Resources");

            if (!File.Exists("Resources/config.json"))
                File.WriteAllText("Resources/config.json", JsonConvert.SerializeObject(bot, Formatting.Indented));
            else
                bot = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("Resources/config.json"));
        }

        public struct BotConfig
        {
            public string DiscordBotToken;
        }
    }
}
