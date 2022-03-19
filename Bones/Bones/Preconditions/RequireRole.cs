using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bones.Preconditions
{
    class RequireRoleSilently : PreconditionAttribute
    {
        private readonly string roleName;

        public RequireRoleSilently(string name) => roleName = name;

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User is SocketGuildUser gUser)
            {
                if (gUser.Roles.Any(r => r.Name == roleName))
                    return await Task.FromResult(PreconditionResult.FromSuccess()).ConfigureAwait(false);
                else
                {
                    return await Task.FromResult(PreconditionResult.FromError($"You must have the {roleName} role to run this command.")).ConfigureAwait(false);
                }
            }
            else
                return await Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command.")).ConfigureAwait(false);
        }
    }
}

