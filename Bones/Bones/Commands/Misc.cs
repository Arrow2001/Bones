using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Bones.Handlers;
using Bones.Preconditions;

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


    }
}
