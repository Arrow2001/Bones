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
using System.Text.RegularExpressions;

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
        public async Task DispalyFM(SocketGuildUser user = null)
        {
                if (user == null)
                  user = (SocketGuildUser)Context.User;

                string footer = "";
                var account = UserAccounts.GetAccount(user);
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
                            .WithIconUrl(user.GetAvatarUrl())
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
        public async Task GetFMTracks([Optional] string timeframe, SocketGuildUser user = null)
        {
            if (user == null)
                user = (SocketGuildUser)Context.User;

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
                var acc = UserAccounts.GetAccount(user);
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
                await Utilities.SendEmbed(Context.Channel, "Last.FM", $"{Context.User.Mention} you need to use a proper timeframe: `7day, 1month, 3month, 6month, 12month, overall`", Color.Red, "", "");

            }
        }

        // Exactly the same as the above command just for the artists instead of the songs.
        [Command("fm artists")]
        public async Task GetTopArtists([Optional] string timeframe, SocketGuildUser user = null)
        {
            if (user == null)
                user = (SocketGuildUser)Context.User;

            var acc = UserAccounts.GetAccount(user);
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
                        .WithIconUrl(user.GetAvatarUrl())
                        .WithName(acc.lastFmUsername)
                        .WithUrl($"https://last.fm/user/" + acc.lastFmUsername))
                    .WithColor(Color.Blue)
                   .WithDescription(artistName.ToString())
                   .WithTitle($"Top Artists | {embedTitle}")
                    .Build());
            } else
            {
                await Utilities.SendEmbed(Context.Channel, "Last.FM", $"{Context.User.Mention} you need to use a proper timeframe: `7day`, `1month`, `3month`, `6month`, `12month`, `overall`", Color.Red, "", "");
            }
        }

        // Get Top Abums. (Ikr who would have seen this one coming :kek:)
        [Command("fm albums")]
        public async Task GetTopAlbums([Optional] string timeframe, SocketGuildUser user = null)
        {
            if (String.IsNullOrWhiteSpace(timeframe))
                timeframe = "7day";

            if (user == null)
                user = (SocketGuildUser)Context.User;
            
            if (timeframe == "7day" || timeframe == "1month" || timeframe == "3month" || timeframe == "6month" || timeframe == "12month" || timeframe == "all")
            {
                var acc = UserAccounts.GetAccount(user);
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
                        .WithIconUrl(user.GetAvatarUrl())
                        .WithName(acc.lastFmUsername)
                        .WithUrl($"https://last.fm/user/" + acc.lastFmUsername))
                    .WithColor(Color.Blue)
                   .WithDescription(AlbumEmbed.ToString())
                   .WithTitle($"Top Albums | {embedTimeframe}")
                   .WithThumbnailUrl(Albums.topalbums.album[0].image[3]["#text"].ToString())
                    .Build());
            } else
            {
                await Utilities.SendEmbed(Context.Channel, "Last.FM", $"{Context.User.Mention} you need to use a proper timeframe: `7day`, `1month`, `3month`, `6month`, `12month`, `overall`", Color.Red, "", "");
            }
        }

        // Display someone's Last.FM Profile
        [Command("fmprofile")]
        [Alias("fm profile")]
        public async Task DisplayFmProfile(SocketGuildUser user = null)
        {
            if (user == null)
                user = (SocketGuildUser)Context.User;

            var account = UserAccounts.GetAccount(user);
            if (account.lastFmUsername == "not set")
            {
                await Utilities.SendEmbed(Context.Channel, "Error", $"{Context.User.Mention} you haven't set up your last.fm within the bot.", Color.Red, "Do .setfm to set your last.fm username", "");
            } else
            {
                string profileUrl = "https://last.fm/user/" + account.lastFmUsername;
                string profileLink = "https://ws.audioscrobbler.com/2.0/?method=user.getinfo&user=" + account.lastFmUsername + "&api_key=" + Config.apiKey.LastFMAPIKey + "&format=json";
                dynamic profile = null;
                using (WebClient client6 = new WebClient())
                    profile = JsonConvert.DeserializeObject(client6.DownloadString(profileLink));

                // Get date of when they joined last.fm
                double unixTime = profile.user.registered["#text"];
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(unixTime).ToLocalTime();

                await Context.Channel.SendMessageAsync(null, false, new EmbedBuilder()
                    .WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(user.GetAvatarUrl())
                        .WithName(account.lastFmUsername)
                        .WithUrl(profileUrl))
                    .WithColor(Color.Blue)
                    .WithFooter($"Total plays: {profile.user.playcount.ToString()}")
                   .AddField("Profile", $"[Link]({profileUrl})", inline: true)
                   .AddField("Registered", $"{dateTime.ToString("dd/MM//yyyy")}", inline: true)
                   .AddField("Country", $"{profile.user.country.ToString()}", inline: true)
                   .WithThumbnailUrl(profile.user.image[3]["#text"].ToString())
                    .Build());
            }
        }

        // Show last 15 tracks played
        [Command("fm recent")]
        public async Task ShowRecentlyPlayed(SocketGuildUser user = null)
        {
            if (user == null)
                user = (SocketGuildUser)Context.User;

            var account = UserAccounts.GetAccount(user);
            string RecentTracks = "http://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user=" + account.lastFmUsername + "&api_key=" + Config.apiKey.LastFMAPIKey + "&format=json";
            dynamic stuff = null;
            using (WebClient client7 = new WebClient())
                stuff = JsonConvert.DeserializeObject(client7.DownloadString(RecentTracks));

            StringBuilder songs = new StringBuilder();
            for (int i = 0; i < 15; i++)
            {
                songs.Append($"**{stuff.recenttracks.track[i].artist["#text"].ToString()}** - **{stuff.recenttracks.track[i].name.ToString()}**\n");
            }

            await Context.Channel.SendMessageAsync(null, false, new EmbedBuilder()
                    .WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(user.GetAvatarUrl())
                        .WithName($"{user.Nickname ?? user.Username}'s - Recent Tracks"))
                    .WithColor(Color.Blue)
                    .WithFooter($"Total plays: {stuff.recenttracks["@attr"].total.ToString()}")
                    .WithDescription(songs.ToString())
                    .WithThumbnailUrl(stuff.recenttracks.track[0].image[3]["#text"].ToString())
                    .Build());
        }

        // Show artist info
        [Command("artistinfo")]
        [Alias("artist info", "infoartist", "info artist")]
        public async Task GetArtistInfo([Remainder]string artist)
        {
            string artistName = "";
            if (artist.Contains(" "))
            {
                artistName = artist.Replace(" ", "+");
            }
            //Taylor+Swift&api_key=e21d20db54e49075a60b72239c173277&format=json
            string link = "http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&artist=" + artistName + "&api_key=" + Config.apiKey.LastFMAPIKey + "&format=json";
            dynamic artistStuff = null;
            using (WebClient client8 = new WebClient())
                artistStuff = JsonConvert.DeserializeObject(client8.DownloadString(link));
            string summary = artistStuff.artist.bio.summary.ToString();
            string summaryToUse = Regex.Replace(summary, @"<a\b[^>]+>([^<]*(?:(?!</a)<[^<]*)*)</a>", " "); // Get rid of the ugly <a> tag from the json

            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"{artistStuff.artist.name.ToString()}'s Artist Overview");
            embed.AddField("Summary", summaryToUse.ToString());
            embed.AddField("Listeners", $"{artistStuff.artist.stats.playcount.ToString()}");
            embed.AddField("Tags", $"{artistStuff.artist.tags.tag[0].name.ToString()}, {artistStuff.artist.tags.tag[1].name.ToString()}, {artistStuff.artist.tags.tag[2].name.ToString()}");
            embed.WithColor(Color.Blue);
            embed.WithFooter($"");

            await ReplyAsync("", false, embed.Build());
        }
    }
}
