﻿using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;

namespace Siotrix.Discord.Moderation
{
    public class KickModule : ModuleBase<SocketCommandContext>
    {
        [Name("Moderator")]
        [Command("kick")]
        public async Task KickAsync(SocketGuildUser user)
        {
            await user.KickAsync();
            await ReplyAsync("👍");
        }

        [Command("kick")]
        public async Task KickAsync(SocketUser user, int prunedays = -1)
        {
            int prune = prunedays == -1 ? 0 : prunedays;
            await Context.Guild.AddBanAsync(user, prune);
            await Context.Guild.RemoveBanAsync(user);
            await ReplyAsync("👍");
        }
    }
}
