﻿using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Siotrix.Discord.Developer
{
    [Name("Developer")]
    [Summary("A .Net reference utility for Siotrix Developers.")]
    public class CsDocsModule : ModuleBase<SocketCommandContext>
    {
        [Command("csdocs")]
        [Summary("Shows class/method reference from the new unified .Net reference.")]
        [Remarks(" <text> - text to search the microsoft api reference.")]
        [MinPermissions(AccessLevel.BotOwner)]
        public async Task GetDocumentationAsync([Remainder] string term)
        {
            var response = await new CsDocsService().GetDocumentationResultsAsync(term);
            var gColor = await Context.GetGuildColorAsync();
            var embedCount = 0;

            foreach (var res in response.Results.Take(3).OrderBy(x => x.DisplayName))
            {
                embedCount++;

                var builder = new EmbedBuilder()
                    .WithColor(GuildEmbedColorExtensions.ConvertStringtoColorObject(gColor.ColorHex))
                    .WithTitle($"{res.ItemKind}: {res.DisplayName}")
                    .WithUrl(res.Url)
                    .WithDescription(res.Description);

                if (embedCount == 3)
                {
                    builder.WithFooter(
                        new EmbedFooterBuilder().WithText(
                            $"{embedCount}/{response.Results.Count} https://docs.microsoft.com/dotnet/api/?term={term}")
                    );
                    builder.Footer.Build();
                }
                builder.Build();
                await ReplyAsync("", embed: builder);
            }
        }
    }
}