﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Siotrix.Discord
{
    [Name("Admin")]
    [Group("blacklist")]
    [Summary("Blacklist module to prevent a user from interacting with Siotrix.")]
    public class BlacklistModule : ModuleBase<SocketCommandContext>
    {
        [Command("list")]
        [Summary("List all users currently blacklisted in the guild.")]
        [Remarks("(username)")]
        [RequireContext(ContextType.Guild)]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task BlacklistListAsync()
        {
            var gColor =  await Context.GetGuildColorAsync();
            var title = $"Blacklistk Information for {Context.Guild.Name}";

            var blacklist = await BlacklistExtensions.GetBlacklistUsersAsync(Context.Guild);
            if (!HasTags(Context.Guild, blacklist)) return;

            foreach (var user in blacklist)
            {
                var currentUser = Context.Guild.GetUser(user.UserId.ToUlong());
                if (currentUser != null && currentUser.Username != user.Username)
                    await BlacklistExtensions.SetBlacklistNameAsync(user, currentUser);
            }

            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithColor(GuildEmbedColorExtensions.ConvertStringtoColorObject(gColor.ColorHex))
                .WithTitle($"Users blacklisted in {Context.Guild}")
                .WithDescription(string.Join(", ", blacklist.Select(x => x.Username)));

            await ReplyAsync("", embed: builder);
        }

        [Command("add")]
        [Summary("Blacklist a user from using any Siotrix commands in guild.")]
        [Remarks("(username)")]
        [RequireContext(ContextType.Guild)]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task BlacklistAddAsync(SocketGuildUser user)
        {
            if (user.IsBot && user.Id == SiotrixConstants.BotId)
            {
                await ReplyAsync($"You cannot add me to the blacklist.");
                return;
            }

            var blacklistedUser = await BlacklistExtensions.GetBlacklistAsync(Context.Guild.Id, user.Id);

            if (blacklistedUser != null)
            {
                await ReplyAsync($"I cannot add {user.Mention} because they are already on the blacklist.");
                return;
            }

            await BlacklistExtensions.CreateBlacklistUserAsync(Context, user);
            await ReplyAsync(
                $"{user.Mention} has officially been blacklisted from using Siotrix commands in {Context.Guild.Name}.");
            var channelToggle = await LogsToggleExtensions.GetLogToggleAsync(Context.Guild.Id, "blacklist");
            var logToggled = await LogsToggleExtensions.GetLogChannelAsync(Context.Guild.Id);

            if (logToggled.IsActive && channelToggle != null)
            {
                var logChannel = Context.Guild.GetChannel(logToggled.ChannelId.ToUlong()) as ISocketMessageChannel;
                var builder = new EmbedBuilder()
                    .WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(user.GetAvatarUrl())
                        .WithName(user.Id + " has been blacklisted."))
                    .WithColor(new Color(0, 0, 0));
                await logChannel.SendMessageAsync(user.Mention, false, builder.Build());
            }
        }

        [Command("remove")]
        [Summary("Re-authorize a blacklisted user to use Siotrix again.")]
        [Remarks("(username)")]
        [RequireContext(ContextType.Guild)]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task BlacklistRemoveAsync(SocketGuildUser user)
        {
            var blacklistedUser = await BlacklistExtensions.GetBlacklistAsync(Context.Guild.Id, user.Id);

            if (blacklistedUser == null)
            {
                await ReplyAsync($"I cannot remove {user.Mention} because they are not on the blacklist.");
                return;
            }

            await BlacklistExtensions.DeleteBlacklistUserAsync(blacklistedUser);
            await ReplyAsync(
                $"{user.Mention} has been removed from the blacklist, and can use Siotrix commands again in {Context.Guild.Name}.");
            var channelToggle = await LogsToggleExtensions.GetLogToggleAsync(Context.Guild.Id, "deblacklist");
            var logToggled = await LogsToggleExtensions.GetLogChannelAsync(Context.Guild.Id);

            if (logToggled.IsActive && channelToggle != null)
            {
                var logChannel = Context.Guild.GetChannel(logToggled.ChannelId.ToUlong()) as ISocketMessageChannel;
                var builder = new EmbedBuilder()
                    .WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(user.GetAvatarUrl())
                        .WithName(user.Id + " has been de-blacklisted."))
                    .WithColor(new Color(0, 0, 0));
                await logChannel.SendMessageAsync(user.Mention, false, builder.Build());
            }

        }

        private bool HasTags(object obj, IEnumerable<DiscordGuildBlacklist> blacklist)
        {
            if (blacklist.Count() == 0)
            {
                var _ = ReplyAsync($"{obj} currently has no blacklist users.");
                return false;
            }
            return true;
        }
    }
}