using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Bones.Handlers;
using Bones.Preconditions;
using Newtonsoft.Json;
using System.IO;
using System;
using Newtonsoft.Json.Linq;
using Discord;
using Bones.Currency.UserData;
using System.Collections.Generic;

namespace Bones.Commands
{
    [RequireContext(ContextType.Guild)]

    public class Misc : ModuleBase<SocketCommandContext>
    {
        // Sends "Pong!" just to make sure the bot is responsive.
        [Command("ping")]
        public async Task PingPong() => await ReplyAsync("Pong!");

        // Allows to see one own's profile picture or another user's pfp.
        [Command("avatar")]
        [Alias("pfp")]
        public async Task GrabProfilePicture() => await ReplyAsync(Context.User.GetAvatarUrl().ToString());

        // Displays a random fact about bones.
        [Command("bonefact")]
        [Alias("bone fact", "bones fact")]
        public async Task SendBonesFact() => await ReplyAsync(ArrayHandler.BonesFacts[Utilities.GetRandomNumber(0, ArrayHandler.BonesFacts.Length)]);

        // Allows Admin & Mods to post a message as the bot in the general chat.
        [Command("say")]
        [RequireRoleSilently("Admins")]
        public async Task SaySomething([Remainder] string words)
        {
            var ch = Context.Channel.Id;
            if (ch == 953797903380533329)
            {
                var client = Context.Client;
                ulong channelID = 953795185417011250;
                var c = client.GetChannel(channelID) as SocketTextChannel;
                await c.SendMessageAsync($"{words}");
                await Context.Channel.SendMessageAsync("Posted!");
            }
        }

        // Show the botinfo stored for the user
        [Command("userinfo")]
        public async Task ShowUserInfo()
        {
            var acc = UserAccounts.GetAccount(Context.User);
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"{Context.User.Username}'s UserInfo");
            embed.AddField($"User ID", acc.UserID);
            embed.AddField($"Bones", acc.Bones);
            embed.AddField($"Last.FM Username", acc.lastFmUsername);
            embed.WithColor(Color.Blue);
            embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            await ReplyAsync("", false, embed.Build());
        }

        public void LoadJson()
        {
            
        }
        public class Episodes
        {
            public string title;
            public string synopsis;
            public string airdate;
            public string director;
            public string image;
        }
        // Get Random Episode
        [Command("tv")]
        public async Task GetRandomBonesEpisode()
        {
            var Episdoe = BonesEpisodesHandler.BonesEps.Episodes[Utilities.GetRandomNumber(0, BonesEpisodesHandler.BonesEps.Episodes.Length)];
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithColor(Color.Blue);
            embed.WithTitle($"Bones: {Episdoe.episodeTitle}");
            embed.WithDescription(Episdoe.synopsis);
            embed.WithImageUrl(Episdoe.image);
            embed.AddField("Air Date", Episdoe.airdate);
            embed.AddField("Director", Episdoe.director);

            await ReplyAsync("", false, embed.Build());
        }
    }
}
