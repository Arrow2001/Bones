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
using Discord.Rest;

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
        public async Task ShowUserInfo(SocketGuildUser user = null)
        {
            if (user == null)
                user = (SocketGuildUser)Context.User;

            var acc = UserAccounts.GetAccount(user);
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"{Context.User.Username}'s UserInfo");
            embed.AddField($"User ID", acc.UserID);
            embed.AddField($"Bones", acc.Bones);
            embed.AddField($"Last.FM Username", acc.lastFmUsername);
            embed.WithColor(Color.Blue);
            embed.WithThumbnailUrl(user.GetAvatarUrl());
            await ReplyAsync("", false, embed.Build());
        }

        // Get Random Episode
        [Command("tv", RunMode = RunMode.Async)]
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

        // help menu
        [Command("help")]
        public async Task HelpMenu()
        {
            RestUserMessage msg;
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Help Menu");
            embed.AddField(".ping", "Replies with 'Pong!'");
            embed.AddField(".avatar", "Shows someone's profile picture");
            embed.AddField(".bonefact", "Sends a random fact about bones");
            embed.AddField(".userinfo", "Shows your information that is saved in the bot");
            embed.AddField(".tv", "Sends a random episode of Bones");
            embed.WithColor(Color.Blue);
            msg = await Context.Channel.SendMessageAsync("", false, embed.Build());

            await msg.AddReactionAsync(new Emoji("◀️"));
            await msg.AddReactionAsync(new Emoji("▶️"));
        }

        // Rock Paper Scissors
        [Command("rps")]
        public async Task RockPaperScissors([Remainder]string choice)
        {
            var acc = UserAccounts.GetAccount(Context.User);
            if (choice == "r" || choice == "p" || choice == "s" || string.IsNullOrWhiteSpace(choice) || string.IsNullOrEmpty(choice))
            {
                string botChoice = ArrayHandler.RPSoptions[Utilities.GetRandomNumber(0, ArrayHandler.RPSoptions.Length)];

                switch (choice) {
                    case "r":
                        choice = "rock";
                        break;
                    case "p":
                        choice = "newspaper";
                        break;
                    case "s":
                        choice = "scissors";
                        break;
                }

                if (botChoice == choice)
                {
                    await Utilities.SendEmbed(Context.Channel, "Rock Paper Scissors", $"You picked :{choice}:, I picked :{botChoice}:.\nIt's a tie!", Color.Gold, "", "");
                } else if (choice == "newspaper" && botChoice == "rock" || choice == "paper" && botChoice == "scissors" || choice == "scissors" && botChoice == "newspaper" || choice == "rock" && botChoice == "scissors")
                {
                    acc.Bones += 10;
                    await Utilities.SendEmbed(Context.Channel, "Rock Paper Scissors", $"You picked :{choice}:, I picked :{botChoice}:.\nYou win! You have been given 10 bones! You now have: {acc.Bones} bones!", Color.Green, "", "");
                    UserAccounts.SaveAccounts();
                } else
                {
                    await Utilities.SendEmbed(Context.Channel, "Rock Paper Scissors", $"You picked :{choice}:, I picked :{botChoice}:.\nYou lost!", Color.Red, "", "");
                }
            } else
            {
                await Utilities.SendEmbed(Context.Channel, "Error", "You need to add `r`, `p` or `s` to give your choice. e.g. `.rps r` would be rock", Color.Red, "", "");
            }
        }
    }
}
