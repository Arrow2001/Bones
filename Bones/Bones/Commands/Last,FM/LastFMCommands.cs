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
        public async Task GetFMTracks([Optional] string timeframe)
        {
            if (String.IsNullOrWhiteSpace(timeframe))
            {
                timeframe = "7day";
            }
            // Just make the options look more readable for the embed.
            string timeframeForEmbed = "";
            switch (timeframe)
            {
                case "7day":
                    timeframeForEmbed = "Last 7 Days";
                    break;
                case "1month":
                    timeframeForEmbed = "Last 30 Days";
                    break;
                case "3month":
                    timeframeForEmbed = "Last 90 Days";
                    break;
                case "6month":
                    timeframeForEmbed = "Last 180 Days";
                    break;
                case "12month":
                    timeframeForEmbed = "Last 365 Days";
                    break;
                case "all":
                    timeframeForEmbed = "All Time";
                    break;
            }

            if (timeframe == "7day" || timeframe == "1month" || timeframe == "3month" || timeframe == "6month" || timeframe == "12month" || timeframe == "all")
            {
                var acc = UserAccounts.GetAccount(Context.User);
                string link = "http://ws.audioscrobbler.com/2.0/?method=user.gettoptracks&user=" + acc.lastFmUsername + "&api_key=" + Config.apiKey.LastFMAPIKey + "&period=" + timeframe + "&format=json";

                dynamic topSongs = null;
                using (WebClient clien3 = new WebClient())
                    topSongs = JsonConvert.DeserializeObject(clien3.DownloadString(link));

                int number = 10;
                StringBuilder Titlesb = new StringBuilder();
                string artistLink = "https://www.last.fm/music/";
                string songLink = "https://www.last.fm/music/";
                for (int i = 0; i < number; i++)
                {
                    string artistname = topSongs.toptracks.track[i].artist.name;
                    string songname = topSongs.toptracks.track[i].name;
                    string artistLinkName = artistname.Replace(" ", "+");
                    string songLinkName = songname.Replace(" ", "+");
                    Titlesb.Append($"{i + 1}. [{artistname}]({artistLink + artistLinkName}) - [{songname}]({songLink + artistLinkName}/_/{songLinkName}): **{topSongs.toptracks.track[i].playcount.ToString("#,##0")}** plays\n");
                }

                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle($"Top Tracks | {timeframeForEmbed}");
                embed.WithColor(Color.Blue);
                embed.WithDescription($"{Titlesb}");

                await ReplyAsync("", false, embed.Build());
            }
            else
            {
                await Utilities.SendEmbed(Context.Channel, "Last.FM", $"{Context.User.Mention} you need to use a proper timeframe: `1month, 3month, 6month, 12month, overall`", Color.Red, "", "");

            }
        }

        // Exactly the same as the above command just for the artists instead of the songs.
        [Command("fm artists")]
        public async Task GetTopArtists([Optional] string timeframe)
        {
            var acc = UserAccounts.GetAccount(Context.User);
            string embedTitle = "";
            // if timeframe isn't provided, it just shows the weekly stats.
            if (String.IsNullOrWhiteSpace(timeframe))
            {
                timeframe = "7day";
            }
            if (timeframe == "7day" || timeframe == "1month" || timeframe == "3month" || timeframe == "6month" || timeframe == "12month" || timeframe == "all")
            {
                // Swicth case part just so the embed is readable
                switch (timeframe)
                {
                    case "7day":
                        embedTitle = "Last 7 Days";
                        break;
                    case "1month":
                        embedTitle = "Last 30 Days";
                        break;
                    case "3month":
                        embedTitle = "Last 60 Days";
                        break;
                    case "6month":
                        embedTitle = "Last 180 Days";
                        break;
                    case "12month":
                        embedTitle = "Last 365 Days";
                        break;
                    case "all":
                        embedTitle = "All Time";
                        break;
                }

                string lastFMLink = "http://ws.audioscrobbler.com/2.0/?method=user.gettopartists&user=" + acc.lastFmUsername + "&api_key=86a285eebdc6aafec7360e6f2c02250e&period=" + timeframe + "&format=json";
                dynamic topArtists = null;
                using (WebClient client4 = new WebClient())
                    topArtists = JsonConvert.DeserializeObject(client4.DownloadString(lastFMLink));

                // starts a fixed loop to get the first 10 artists from the json link
                int number = 10;
                StringBuilder artistName = new StringBuilder();
                string artistLink = "https://www.last.fm/music/";

                for (int i = 0; i < number; i++)
                {
                    string artist = topArtists.topartists.artist[i].name;
                    string LinkToArtist = artist.Replace(" ", "+");
                    artistName.Append($"{i + 1}. [{artist}]({artistLink + LinkToArtist}): **{topArtists.topartists.artist[i].playcount}** plays\n");
                }

                await Context.Channel.SendMessageAsync(null, false, new EmbedBuilder()
                    .WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(Context.User.GetAvatarUrl())
                        .WithName(acc.lastFmUsername)
                        .WithUrl($"https://last.fm/user/" + acc.lastFmUsername))
                    .WithColor(Color.Blue)
                   .WithDescription(artistName.ToString())
                   .WithTitle($"Top Artists | {embedTitle}")
                    .Build());
            } else
            {
                await Utilities.SendEmbed(Context.Channel, "Last.FM", $"{Context.User.Mention} you need to use a proper timeframe: `1month, 3month, 6month, 12month, overall`", Color.Red, "", "");
            }
        }

        // Get Top Abums. (Ikr who would have seen this one coming :kek:)
        [Command("fm albums")]
        public async Task GetTopAlbums([Optional] string timeframe)
        {
            if (String.IsNullOrWhiteSpace(timeframe))
            {
                timeframe = "7day";
            }
            if (timeframe == "7day" || timeframe == "1month" || timeframe == "3month" || timeframe == "6month" || timeframe == "12month" || timeframe == "all")
            {
                var acc = UserAccounts.GetAccount(Context.User);
                string lastFM = "http://ws.audioscrobbler.com/2.0/?method=user.gettopalbums&user=" + acc.lastFmUsername + "&api_key=" + Config.apiKey.LastFMAPIKey + "&format=json&period="+timeframe;
                string embedTimeframe = "";
                dynamic Albums = null;
                using (WebClient client5 = new WebClient())
                    Albums = JsonConvert.DeserializeObject(client5.DownloadString(lastFM));

                // Make Embed Look Nicer
                switch (timeframe)
                {
                    case "7day":
                        embedTimeframe = "Last 7 Days";
                        break;
                    case "1month":
                        embedTimeframe = "Last 30 Days";
                        break;
                    case "3month":
                        embedTimeframe = "Last 90 Days";
                        break;
                    case "6month":
                        embedTimeframe = "Last 180 Days";
                        break;
                    case "12month":
                        embedTimeframe = "Last 365 Days";
                        break;
                    case "all":
                        embedTimeframe = "All Time";
                        break;
                }

                StringBuilder AlbumEmbed = new StringBuilder();
                string lastfmArtist = "https://www.last.fm/music/";
                int number = 10;
                for (int i = 0; i < number; i++)
                {
                    string albumName = Albums.topalbums.album[i].name;
                    string artistName = Albums.topalbums.album[i].artist.name;
                    string ArtistNameToUse = artistName.Replace(" ", "+");
                    string AlbumNameToUse = albumName.Replace(" ", "+");
                    string AlbumLink = "https://www.last.fm/music/" + ArtistNameToUse + "/" + AlbumNameToUse;
                    AlbumEmbed.Append($"{i + 1}. [{artistName}]({lastfmArtist+ArtistNameToUse}) - [{albumName}]({AlbumLink}): **{Albums.topalbums.album[i].playcount}** plays\n");
                }

                await Context.Channel.SendMessageAsync(null, false, new EmbedBuilder()
                    .WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(Context.User.GetAvatarUrl())
                        .WithName(acc.lastFmUsername)
                        .WithUrl($"https://last.fm/user/" + acc.lastFmUsername))
                    .WithColor(Color.Blue)
                   .WithDescription(AlbumEmbed.ToString())
                   .WithTitle($"Top Albums | {embedTimeframe}")
                   .WithThumbnailUrl(Albums.topalbums.album[0].image[3]["#text"].ToString())
                    .Build());
            } else
            {
                await Utilities.SendEmbed(Context.Channel, "Last.FM", $"{Context.User.Mention} you need to use a proper timeframe: `1month, 3month, 6month, 12month, overall`", Color.Red, "", "");
            }
        }
    }
}
