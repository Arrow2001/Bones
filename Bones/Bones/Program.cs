using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Bones
{
    class Program
    {
        public static DiscordSocketClient _client;
        private IServiceProvider _services;

        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (Config.bot.DiscordBotToken == "" || Config.bot.DiscordBotToken == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Verbose });
            _client.Log += Log;
            Console.WriteLine($"{Config.bot.DiscordBotToken.ToString()}");
            await _client.LoginAsync(TokenType.Bot, Config.bot.DiscordBotToken);
            await _client.StartAsync();
            // displays a activity
            await _client.SetGameAsync("Bones & Booth sing Hot Blooded", null, ActivityType.Watching);
            var _handler = new Handlers.EventHandler(_services);
            await _handler.InitialiseAsync(_client);
            await Task.Delay(-1);
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
            return Task.CompletedTask;
        }
    }
}
