using System;
using Discord;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Bones.Currency.UserData;

namespace Bones.Currency
{
    [RequireContext(ContextType.Guild)]
    static class BoneHandler
    {
        private static readonly Color Blue = new Color(36, 134, 244);
        private const string BoneIcon = "https://i.imgur.com/fpduHD6.png";
        private static async Task PrintEmbed(ISocketMessageChannel channel, string description) => await Utilities.SendEmbed(channel, "Bones", description, Blue, "", "");
        private static async Task PrintEmbedNoFooter(ISocketMessageChannel channel, string description) => await Utilities.SendEmbed(channel, "Bones", description, Blue, "", "");


        // Give Bones
        public static async Task GiveBones(SocketCommandContext context, SocketGuildUser sender, SocketGuildUser reciever, int amount)
        {
            var senderAccount = UserAccounts.GetAccount(sender);
            var recieverAccount = UserAccounts.GetAccount(reciever);

            if (amount < 1)
            {
                await PrintEmbed(context.Channel, $"{sender.Mention}, keep continuing on like this and your experiments won't end well. You need to enter an amount higher than 0!").ConfigureAwait(false);
                return;
            }
            else if (amount > senderAccount.Bones)
            {
                await PrintEmbed(context.Channel, $"{sender.Mention}, this is the kind of thing that will make you get sent to prison for a murder you didn't commit! You don't have that many Bones!").ConfigureAwait(false);
                return;
            }
            senderAccount.Bones -= amount;
            recieverAccount.Bones += amount;
            UserAccounts.SaveAccounts();
            await PrintEmbedNoFooter(context.Channel, $"{sender.Mention} has gave {reciever.Mention} {amount} bones!").ConfigureAwait(false);
        }

        // View how many bones you have
        public static async Task DisplayBones(SocketGuildUser user, ISocketMessageChannel channel)
        {
            await Utilities.SendEmbed(channel, "Bones", $"{user.Nickname ?? user.Username} has {UserAccounts.GetAccount(user).Bones.ToString("#,##0")} bones!", Blue, "", BoneIcon);
        }

        // Adjust how many bones someone has
        public static void AdjustBones(SocketGuildUser user, int amount)
        {
            var account = UserAccounts.GetAccount(user);
            account.Bones += amount;
            if (account.Bones < 0)
                account.Bones = 0;
            UserAccounts.SaveAccounts();
        }

        // Bone store
        public static async Task DisplayBoneStore(SocketCommandContext context, SocketGuildUser user, ISocketMessageChannel channel)
        {
            await Utilities.SendEmbed(channel, "Bones Store", $"1. Squintern Hoisted Role - 2000 Bones\n2. Colour Roles Menu", Blue, $"You have {UserAccounts.GetAccount(user).Bones.ToString("#,##0")} Bones. Do .store buy to select items.", BoneIcon);
        }

        public static async Task DisplayBonesColourRolesMenu(SocketCommandContext context, SocketGuildUser user, ISocketMessageChannel channel)
        {
            await Utilities.SendEmbed(channel, "Bones Store", $"3. Blue - 1000 Bones\n4. Red - 1000 Bones\n5. Pink - 1000 Bones\n6. Green - 1000 Bones\n7. Yellow - 1000 Bones\n8. Orange - 1000 Bones", Blue, $"You have {UserAccounts.GetAccount(user).Bones.ToString("#,##0")} Bones. Do .store buy to select items.", BoneIcon);
        }

        public static async Task StoreFunctions(SocketCommandContext context, SocketGuildUser user, ISocketMessageChannel channel, int option)
        {
            var userAccount = UserAccounts.GetAccount(user);
            var role = context.Guild.Roles.FirstOrDefault(x => x.Name == "Squintern");
            var blueRole = context.Guild.Roles.FirstOrDefault(x => x.Name == "Blue");
            var redRole = context.Guild.Roles.FirstOrDefault(x => x.Name == "Red");
            var pinkRole = context.Guild.Roles.FirstOrDefault(x => x.Name == "Pink");
            var greenRole = context.Guild.Roles.FirstOrDefault(x => x.Name == "Green");
            var yellowRole = context.Guild.Roles.FirstOrDefault(x => x.Name == "Yellow");
            var orangeRole = context.Guild.Roles.FirstOrDefault(x => x.Name == "Orange");

            if (user.Roles.Contains(blueRole) || user.Roles.Contains(redRole) || user.Roles.Contains(pinkRole) || user.Roles.Contains(pinkRole) || user.Roles.Contains(greenRole) || user.Roles.Contains(yellowRole) || user.Roles.Contains(orangeRole))
            {
                await Utilities.SendEmbed(channel, "Error!", $"{user.Mention} you already have a colour role! You need to sell it to be able to grab another! Do .sell colour to get a refund!", Color.Red, "", "");
            }
            else
            {
                switch (option)
                {
                    case 1: // Squintern
                        await user.AddRoleAsync(role);
                        userAccount.Bones -= 2000;
                        UserAccounts.SaveAccounts();
                        await Utilities.SendEmbed(channel, "Bones Store", $"{user.Mention} has bought the Squintern role for 2000 Bones!", Blue, "", BoneIcon);
                        break;
                    case 2: // Show colour menu
                        await DisplayBonesColourRolesMenu(context, (SocketGuildUser)context.User, context.Channel);
                        break;
                    case 3: // Blue colour role
                        await user.AddRoleAsync(blueRole);
                        userAccount.Bones -= 1000;
                        UserAccounts.SaveAccounts();
                        await Utilities.SendEmbed(channel, "Bones Store", $"{user.Mention} has bought the blue colour role for 1000 Bones!", Blue, "", BoneIcon);
                        break;
                    case 4: // Red colour role
                        await user.AddRoleAsync(redRole);
                        userAccount.Bones -= 1000;
                        UserAccounts.SaveAccounts();
                        await Utilities.SendEmbed(channel, "Bones Store", $"{user.Mention} has bought the red colour role for 1000 Bones!", Blue, "", BoneIcon);
                        break;
                    case 5: // Pink colour role
                        await user.AddRoleAsync(pinkRole);
                        userAccount.Bones -= 1000;
                        UserAccounts.SaveAccounts();
                        await Utilities.SendEmbed(channel, "Bones Store", $"{user.Mention} has bought the pink colour role for 1000 Bones!", Blue, "", BoneIcon);
                        break;
                    case 6: // Green colour role
                        await user.AddRoleAsync(greenRole);
                        userAccount.Bones -= 1000;
                        UserAccounts.SaveAccounts();
                        await Utilities.SendEmbed(channel, "Bones Store", $"{user.Mention} has bought the green colour role for 1000 Bones!", Blue, "", BoneIcon);
                        break;
                    case 7: // Yellow colour role
                        await user.AddRoleAsync(yellowRole);
                        userAccount.Bones -= 1000;
                        UserAccounts.SaveAccounts();
                        await Utilities.SendEmbed(channel, "Bones Store", $"{user.Mention} has bought the yellow colour role for 1000 Bones!", Blue, "", BoneIcon);
                        break;
                    case 8: // Orange colour role
                        await user.AddRoleAsync(orangeRole);
                        userAccount.Bones -= 1000;
                        UserAccounts.SaveAccounts();
                        await Utilities.SendEmbed(channel, "Bones Store", $"{user.Mention} has bought the orange colour role for 1000 Bones!", Blue, "", BoneIcon);
                        break;
                }
            }
        }
    }
}