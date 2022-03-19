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

namespace Bones.Commands.Last_FM
{
    [RequireContext(ContextType.Guild)]
    public class LastFMCommands : ModuleBase<SocketCommandContext>
    {

        // Trigger Warning: This code is horrific and there definitely would be a betetr way to do this.

        // Set FM
        [Command("fm set")]
        [Alias("set fm")]
        public async Task setFM([Remainder]string username)
        {
            var acc = UserAccounts.GetAccount(Context.User);
            acc.lastFmUsername = username;
            UserAccounts.SaveAccounts();
            await Utilities.SendEmbed(Context.Channel, "Last.FM", $"{Context.User.Mention}, you have now set your last.fm username to: {username}", Discord.Color.Blue, "", "");
        }

        // Show Now Playing
        [Command("fm")]
        public async Task DispalyFM()
        {
            string footer = "";
            var account = UserAccounts.GetAccount(Context.User);
            string link1 = "http://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user=";
            dynamic stuff = null;
            using (WebClient client = new WebClient())
                stuff = JsonConvert.DeserializeObject(client.DownloadString(link1 + account.lastFmUsername + $"&api_key={Config.apiKey.LastFMAPIKey}&format=json"));
            
            if (stuff.recenttracks.track[0]["@attr"].nowplaying == "true")
            {
                 footer = "Now Playing";
            } else
            {
                footer = "Most Recently Played"; // need to fix
            }
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
                .AddField("Track", $"[{stuff.recenttracks.track[0].name.ToString()}]({stuff.recenttracks.track[0].url.ToString()})", true)
                .AddField("Artist", $"[{stuff.recenttracks.track[0].artist["#text"].ToString()}]({LinkToUse})", true)
                .WithThumbnailUrl(stuff.recenttracks.track[0].image[3]["#text"].ToString())
                .WithFooter(footer)
                .Build()) ;
        }
    }
}
