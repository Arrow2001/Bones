using System;
using Discord;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Bones.Currency;
using Bones.Currency.UserData;
using Bones.Preconditions;
using System;
using Discord;
using System.Linq;
using System.Text;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bones.Currency.UserData;

namespace Bones.Commands
{
    [RequireContext(ContextType.Guild)]
    public class CurrencyCommands : ModuleBase<SocketCommandContext>
    {
        private const string BoneIcon = "https://i.imgur.com/fpduHD6.png";

        // Spawn Bones
        [Command("spawn")]
        [RequireRoleSilently("Admins")]
        public async Task spawnBones(SocketGuildUser user, int amount)
        {
            BoneHandler.AdjustBones(user, amount);
            await Utilities.SendEmbed(Context.Channel, "Bones", $"{user.Mention} has been spawned {amount} bones!", Color.Blue, "", "https://i.imgur.com/fpduHD6.png");
        }

        // View the number of bones you have
        [Command("bones")]
        public async Task ViewBones(SocketGuildUser user = null)
        {
            if (user == null)
                user = (SocketGuildUser)Context.User;
            await BoneHandler.DisplayBones(user, Context.Channel);
        }

        // Gamble bones
        [Command("gamble")]
        public async Task GambleBones(int amount)
        {
            Random rnd = new Random();
            int WinOrLose = rnd.Next(0, 100);
            var UserAccount = UserAccounts.GetAccount(Context.User);
            if (WinOrLose <= 50)
            {
                UserAccount.Bones -= amount;
                UserAccounts.SaveAccounts();
                await Utilities.SendEmbed(Context.Channel, "You lost!", $"You rolled {WinOrLose}/100! You now have {UserAccount.Bones}", Color.Red, "", BoneIcon);
            } else if (WinOrLose > 50)
            {
                UserAccount.Bones += amount;
                UserAccounts.SaveAccounts();
                await Utilities.SendEmbed(Context.Channel, "You won!", $"You rolled {WinOrLose}/100! You now have {UserAccount.Bones}", Color.Blue, "", BoneIcon);
            }    
        }

        [Command("sell colour")]
        [Alias("sell color")]
        public async Task SellColour()
        {
            var account = UserAccounts.GetAccount(Context.User);
            var user = (SocketGuildUser)Context.User;
            foreach (var role in user.Roles)
            {
                if (role.Id == 954514522603925545 || role.Id == 954514618145964062 || role.Id == 954514669102592011 || role.Id == 954514697061802074 || role.Id == 954514732247826472 || role.Id == 954514768880889948)
                {
                    await user.RemoveRoleAsync(role);
                    account.Bones += 1000;
                    UserAccounts.SaveAccounts();
                }
            }
            await Utilities.SendEmbed(Context.Channel, "Bones Refund!", $"{Context.User.Mention} has sold their colour role for 1000 Bones!", Color.Blue, "", BoneIcon);
        }

        // Bone Store
        [Command("store")]
        public async Task ShowBoneStore() => await BoneHandler.DisplayBoneStore(Context, (SocketGuildUser)Context.User, Context.Channel);

        [Command("store buy")]
        public async Task BuySomethingFromTheStore(int option) => await BoneHandler.StoreFunctions(Context, (SocketGuildUser)Context.User, Context.Channel, option);
    }
}
