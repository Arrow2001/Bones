using System;
using Discord;
using System.Linq;
using Discord.Commands;
using System.Reflection;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Bones.Handlers
{
    class EventHandler
    {
        DiscordSocketClient _client;
        CommandService _service;
        readonly IServiceProvider serviceProdiver;

        public EventHandler(IServiceProvider services) => serviceProdiver = services;

        public async Task InitialiseAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();


            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProdiver);

            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += HandleNewMember;

            _service.Log += Log;

        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private async Task HandleNewMember(SocketGuildUser user)
        {
            ulong bonesChat = 953795185417011250;
            await user.Guild.GetTextChannel(bonesChat).SendMessageAsync($"Welcome to the Bones discord server!, {user.Mention}!\n");
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            SocketUserMessage msg = s as SocketUserMessage;
            if (msg == null || msg.Author.IsBot) return;

            var context = new SocketCommandContext(_client, msg);

            int argPos = 0;
            if (msg.HasStringPrefix(".", ref argPos))
                await _service.ExecuteAsync(context, argPos, serviceProdiver, MultiMatchHandling.Exception);

            string m = msg.Content.ToLower();
        }
    }
}