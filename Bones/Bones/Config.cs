using System.IO;
using Newtonsoft.Json;
using Bones.Handlers;
namespace Bones
{
    static class Config
    {
        public static readonly BotConfig bot;
        public static readonly BotConfig apiKey;


        static Config()
        {
            BonesEpisodesHandler.BoneSetup();

            if (!Directory.Exists("Resources"))
                Directory.CreateDirectory("Resources");

            if (!File.Exists("Resources/config.json"))
                File.WriteAllText("Resources/config.json", JsonConvert.SerializeObject(bot, Formatting.Indented));
            else
            {
                bot = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("Resources/config.json"));
                apiKey = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("Resources/config.json"));
            }

        }

        public struct BotConfig
        {
            public string DiscordBotToken;
            public string LastFMAPIKey;
        }
    }
}
