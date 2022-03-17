using System;
using Discord;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Bones.Commands
{
    [RequireContext(ContextType.Guild)]

    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingPong() => await ReplyAsync("Pong!");

        [Command("avatar")]
        [Alias("pfp")]
        public async Task GrabProfilePicture() => await ReplyAsync(Context.User.GetAvatarUrl().ToString());
    }
}
