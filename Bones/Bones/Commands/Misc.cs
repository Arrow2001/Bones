using System;
using Discord;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Bones.Handlers;

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

        [Command("bonefact")]
        [Alias("bone fact", "bones fact")]
        public async Task SendBonesFact() => await ReplyAsync(ArrayHandler.BonesFacts[Utilities.GetRandomNumber(0, ArrayHandler.BonesFacts.Length)]);
    }
}
