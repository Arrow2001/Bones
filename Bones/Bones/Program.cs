using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Bones
{
    class Program
    {

        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();
        public static DiscordSocketClient _client;

        private readonly IServiceProvider services;
        public async Task StartAsync()
        {
            if (Config.bot.DiscordBotToken == "" || Config.bot.DiscordBotToken == null) return;
            if (Config.apiKey.LastFMAPIKey == "" || Config.apiKey.LastFMAPIKey == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Verbose, AlwaysDownloadUsers = true, MessageCacheSize = 100, GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers | GatewayIntents.GuildMessages });
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, Config.bot.DiscordBotToken);
            await _client.StartAsync();

            
            // displays a activity
            await _client.SetGameAsync("Hot Blooded", null, ActivityType.Listening);


            var _handler = new Handlers.EventHandler(services);
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