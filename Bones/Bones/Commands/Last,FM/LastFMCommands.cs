using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Bones.Handlers;
using Bones.Preconditions;
using Newtonsoft.Json;
using System.IO;
using System;
using Newtonsoft.Json.Linq;
using Bones.Currency.UserData;
using System.Net;
using Discord;
using System.Runtime.InteropServices;
using System.Text;

namespace Bones.Commands.Last_FM
{
    [RequireContext(ContextType.Guild)]
    public class LastFMCommands : ModuleBase<SocketCommandContext>
    {

        // Trigger Warning: This code is horrific and there definitely would be a betetr way to do this.

        // Set FM
        [Command("fm set")]
        [Alias("set fm", "setfm", "fmset")]
        public async Task setFM([Remainder] string username)
        {
            var acc = UserAccounts.GetAccount(Context.User);
            if (acc.lastFmUsername != "not set")
            {
                await Utilities.SendEmbed(Context.Channel, "Last.FM", $"{Context.User.Mention} you already have set your last.fm username", Color.Blue, "Do .clearfm to remove it.", "");
            }
            else
            {
                acc.lastFmUsername = username;
                UserAccounts.SaveAccounts();
                await Utilities.SendEmbed(Context.Channel, "Last.FM", $"{Context.User.Mention}, you have now set your last.fm username to: {username}", Discord.Color.Blue, "", "");

            }
        }

        // Show Now Playing
        [Command("fm")]
        public async Task DispalyFM()
        {
            string footer = "";
            var account = UserAccounts.GetAccount(Context.User);
            if (account.lastFmUsername == "not set")
            {
                await Utilities.SendEmbed(Context.Channel, "Last.FM", $"{Context.User.Mention}, you don't have your Last.FM set up.", Color.Blue, "Do .setfm to set up your last.fm", "");
            }
            else
            {
                string link1 = "http://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user=";
                dynamic stuff = null;
                using (WebClient client = new WebClient())
                    stuff = JsonConvert.DeserializeObject(client.DownloadString(link1 + account.lastFmUsername + $"&api_key={Config.apiKey.LastFMAPIKey}&format=json"));

                if (stuff.recenttracks.track[0]["@attr"] != null)
                {
                    footer = "Now Playing";
                }
                else
                {
                    footer = "Most Recent Track";
                }

                // Total Scrobbles: http://ws.audioscrobbler.com/2.0/?method=user.getinfo&user=iain2001&api_key=e21d20db54e49075a60b72239c173277&format=json
                dynamic totalScrobles = null;
                using (WebClient client2 = new WebClient())
                    totalScrobles = JsonConvert.DeserializeObject(client2.DownloadString("http://ws.audioscrobbler.com/2.0/?method=user.getinfo&user=" + account.lastFmUsername + "&api_key=" + Config.apiKey.LastFMAPIKey + "&format=json"));

                string artistLink = "https://last.fm/music/";
                string bLink = stuff.recenttracks.track[0].artist["#text"];
                string cLink = bLink.Replace(" ", "+");
                string LinkToUse = artistLink + cLink;
                await Context.Channel.SendMessageAsync(null, false, new EmbedBuilder()
                    .WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(Context.User.GetAvatarUrl())
                        .WithName(account.lastFmUsername)
                        .WithUrl($"https://last.fm/user/" + account.lastFmUsername))
                    .WithColor(Color.Green)
                    .AddField("Artist", $"[{stuff.recenttracks.track[0].artist["#text"].ToString()}]({LinkToUse})", true)
                    .AddField("Track", $"[{stuff.recenttracks.track[0].name.ToString()}]({stuff.recenttracks.track[0].url.ToString()})", true)
                    .WithThumbnailUrl(stuff.recenttracks.track[0].image[3]["#text"].ToString())
                    .WithFooter($"{footer} | Total Scrobbles: {totalScrobles.user.playcount.ToString()}")
                    .Build());
            }
        }

        // Clear Last.FM
        [Command("clearfm")]
        [Alias("clear fm", "fm clear", "fmclear")]
        public async Task ClearFM()
        {
            var account = UserAccounts.GetAccount(Context.User);
            account.lastFmUsername = "not set";
            UserAccounts.SaveAccounts();
            await Utilities.SendEmbed(Context.Channel, "Last.FM", $"{Context.User.Mention} you have cleared your last.fm account", Color.Blue, "Do .setfm to set you last.fm username.", "");
        }

        // Get FM Tracks from a set time
        [Command("fm tracks")]
        public async Task GetFMTracks([Optional]string timeframe)
        {
            if (String.IsNullOrWhiteSpace(timeframe))
            {
                timeframe = "7day";
            }
            if (timeframe == "7day" || timeframe == "1month" || timeframe == "3month" || timeframe == "6month" || timeframe == "12month" || timeframe == "all")
            {
                var acc = UserAccounts.GetAccount(Context.User);
                string link = "http://ws.audioscrobbler.com/2.0/?method=user.gettoptracks&user=" + acc.lastFmUsername + "&api_key=" + Config.apiKey.LastFMAPIKey + "&period=" + timeframe + "&format=json";

                dynamic topSongs = null;
                using (WebClient clien3 = new WebClient())
                    topSongs = JsonConvert.DeserializeObject(clien3.DownloadString(link));

                int number = 10;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < number; i++)
                {
                    sb.Append($"{topSongs.toptracks.track[i].name}\n");
                }
                await Context.Channel.SendMessageAsync(sb.ToString());

                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle($"Top Tracks | {timeframe}");
                embed.WithColor(Color.Blue);
                embed.WithDescription($"{sb.ToString()}");
            }
            else
            {               
                await Utilities.SendEmbed(Context.Channel, "Last.FM", $"{Context.User.Mention} you need to use a proper timeframe: `1month, 3month, 6month, 12month, overall`", Color.Blue, "", "");

            }
        }
    }
}
