﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Siotrix.Discord
{
    public static class MuteExtensions
    {
        public enum MuteType
        {
            Voice,
            Chat,
            All
        }

        public static readonly OverwritePermissions DenyOverwrite =
            new OverwritePermissions(sendMessages: PermValue.Deny, attachFiles: PermValue.Deny);

        public static ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, Timer>> UnmuteTimers { get; }
            = new ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, Timer>>();

        public static event Action<IGuildUser, MuteType, SocketCommandContext, int, bool> UserMuted = delegate { };
        public static event Action<IGuildUser, MuteType, SocketCommandContext, bool> UserUnmuted = delegate { };


        public static async Task<IRole> GetMuteRole(IGuild guild)
        {
            var roleName = GetMuteRoleName(guild.Id.ToLong());

            var muteRole = guild.Roles.FirstOrDefault(r => r.Name == roleName);
            if (muteRole == null)
                try
                {
                    muteRole = await guild.CreateRoleAsync(roleName, GuildPermissions.None).ConfigureAwait(false);
                }
                catch
                {
                    muteRole = guild.Roles.FirstOrDefault(r => r.Name == roleName) ?? await guild
                                   .CreateRoleAsync(roleName, GuildPermissions.None).ConfigureAwait(false);
                }

            foreach (var toOverwrite in await guild.GetTextChannelsAsync())
                try
                {
                    if (!toOverwrite.PermissionOverwrites.Select(x => x.Permissions).Contains(DenyOverwrite))
                    {
                        await toOverwrite.AddPermissionOverwriteAsync(muteRole, DenyOverwrite).ConfigureAwait(false);
                        await Task.Delay(200).ConfigureAwait(false);
                    }
                }
                catch
                {
                    // ignored
                }

            return muteRole;
        }

        public static string GetMuteRoleName(long guildId)
        {
            string name = null;
            if (guildId <= 0) return null;
            using (var db = new LogDatabase())
            {
                try
                {
                    var result = db.Gmuteroles.FirstOrDefault(x => x.GuildId == guildId);
                    if (result != null)
                        name = result.MuteRoleName;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return name;
        }

        public static bool SaveMuteUser(IGuildUser user, int minutes)
        {
            var isSave = false;
            using (var db = new LogDatabase())
            {
                try
                {
                    var result =
                        db.Gmutelists.Where(x => x.GuildId == user.Guild.Id.ToLong() && x.UserId == user.Id.ToLong());
                    if (!result.Any())
                    {
                        var record = new DiscordGuildMuteList();
                        record.GuildId = user.Guild.Id.ToLong();
                        record.UserId = user.Id.ToLong();
                        record.MuteTime = minutes;
                        db.Gmutelists.Add(record);
                        isSave = true;
                    }
                    else
                    {
                        var data = result.First();
                        data.MuteTime = minutes;
                        db.Gmutelists.Update(data);
                        isSave = true;
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return isSave;
        }

        public static bool RemoveMuteUser(IGuildUser user)
        {
            var isRemove = false;
            using (var db = new LogDatabase())
            {
                try
                {
                    var result =
                        db.Gmutelists.Where(x => x.GuildId == user.Guild.Id.ToLong() && x.UserId == user.Id.ToLong());
                    if (result.Any())
                    {
                        db.Gmutelists.RemoveRange(result);
                        isRemove = true;
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return isRemove;
        }

        public static async Task TimedMute(IGuildUser user, TimeSpan after, int minutes, SocketCommandContext context,
            bool isAuto)
        {
            await MuteUser(user, minutes, context, isAuto)
                .ConfigureAwait(false); // mute the user. This will also remove any previous unmute timers

            StartUnmuteTimer(user.GuildId, user.Id, after, context); // start the timer
        }

        public static void StopUnmuteTimer(ulong guildId, ulong userId)
        {
            ConcurrentDictionary<ulong, Timer> userUnmuteTimers;
            if (!UnmuteTimers.TryGetValue(guildId, out userUnmuteTimers)) return;

            Timer removed;
            if (userUnmuteTimers.TryRemove(userId, out removed))
                removed.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static async Task MuteUser(IGuildUser usr, int minutes, SocketCommandContext context, bool isAuto)
        {
            await usr.ModifyAsync(x => x.Mute = true).ConfigureAwait(false);
            var muteRole = await GetMuteRole(usr.Guild);
            if (!usr.RoleIds.Contains(muteRole.Id))
                await usr.AddRoleAsync(muteRole).ConfigureAwait(false);
            StopUnmuteTimer(usr.GuildId, usr.Id);
            UserMuted(usr, MuteType.All, context, minutes, isAuto);
        }

        public static void StartUnmuteTimer(ulong guildId, ulong userId, TimeSpan after, SocketCommandContext context)
        {
            //load the unmute timers for this guild
            var userUnmuteTimers = UnmuteTimers.GetOrAdd(guildId, new ConcurrentDictionary<ulong, Timer>());

            //unmute timer to be added
            var toAdd = new Timer(async _ =>
            {
                try
                {
                    await UnmuteUser(context.Guild.GetUser(userId), true, context).ConfigureAwait(false);
                    var caseId = context.GetCaseNumber();
                    CaseExtensions.SaveCaseDataAsync("unmute", caseId, userId.ToLong(), context.Guild.Id.ToLong(),
                        "auto");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }, null, after, Timeout.InfiniteTimeSpan);

            //add it, or stop the old one and add this one
            userUnmuteTimers.AddOrUpdate(userId, key => toAdd, (key, old) =>
            {
                old.Change(Timeout.Infinite, Timeout.Infinite);
                return toAdd;
            });
        }

        public static async Task UnmuteUser(this IGuildUser usr, bool isAuto, SocketCommandContext context)
        {
            var isRemove = RemoveMuteUser(usr);
            if (isRemove)
            {
                StopUnmuteTimer(usr.GuildId, usr.Id);
                try
                {
                    await usr.ModifyAsync(x => x.Mute = false).ConfigureAwait(false);
                }
                catch
                {
                }
                try
                {
                    await usr.RemoveRoleAsync(await GetMuteRole(usr.Guild)).ConfigureAwait(false);
                }
                catch
                {
                    /*ignore*/
                }

                UserUnmuted(usr, MuteType.All, context, isAuto);
            }
        }
    }
}