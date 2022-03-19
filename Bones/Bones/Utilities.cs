using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Bones
{
    static class Utilities
    {
        public static readonly Random getrandom = new Random();
        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) { return getrandom.Next(min, max); }
        }

        public static Embed Embed(string title, string desc, Discord.Color col, string foot, string thURL) => new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(desc)
            .WithColor(col)
            .WithFooter(foot)
            .WithThumbnailUrl(thURL)
            .Build();

        public static async Task SendEmbed(ISocketMessageChannel channel, string title, string description, Discord.Color color, string footer, string thumbnailURL)
        {
            await channel.SendMessageAsync(null, false, Embed(title, description, color, footer, thumbnailURL)).ConfigureAwait(false);
        }
    }
}
