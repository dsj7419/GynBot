﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.EmojiTools;
using Discord.Addons.InteractiveCommands;
using Discord.Commands;

namespace Siotrix.Discord.Admin
{
    [Name("Admin")]
    [Group("settings")]
    [Summary("Various settings for guild to customize Siotrix with.")]
    [RequireContext(ContextType.Guild)]
    public class SettingsModule : ModuleBase<SocketCommandContext>
    {
        private readonly Stopwatch _timer = new Stopwatch();
        private readonly InteractiveService _interactive;

        public SettingsModule(InteractiveService inter)
        {
            _interactive = inter;
        }

        [Command("gfootericon")]
        [Summary("Will list bots current footer icon.")]
        [Remarks(" - no additional arguments needed.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task FooterIconAsync()
        {
            var val = await Context.GetGuildFooterAsync();
            await ReplyAsync(val.FooterIcon);
        }

        [Command("gfootericon")]
        [Summary("Will set bots footer icon.")]
        [Remarks(
            "<url> - url of picture to assign as bot footer icon **note** using keyword reset will reset to Siotrix icon.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task FooterIconAsync(Uri url)
        {
            var val = await Context.GetGuildFooterAsync();

            if (url.ToString().Equals("reset"))
            {
                await GuildEmbedFooter.SetGuildFooterIcon(val, SiotrixConstants.BotFooterIcon);
                await ReplyAsync(SiotrixConstants.BotSuccess);
                return;
            }

            await GuildEmbedFooter.SetGuildFooterIcon(val, url.ToString());
            await ReplyAsync(SiotrixConstants.BotSuccess);
        }

        [Command("gfootertext")]
        [Summary("Will list bots current footer text.")]
        [Remarks(" - no additional arguments needed.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task FooterTextAsync()
        {
            var val = await Context.GetGuildFooterAsync();
            await ReplyAsync(val.FooterText);
        }

        [Command("gfootertext")]
        [Summary("Will set bots footer text.")]
        [Remarks(
            "<text> - text you would like to use on embed footers. **note** using keyword reset will reset to Siotrix footer text.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task FooterTextAsync([Remainder] string txt)
        {
            var val = await Context.GetGuildFooterAsync();

            if (txt.Equals("reset"))
            {
                await GuildEmbedFooter.SetGuildFooterText(val, SiotrixConstants.BotFooterText);
                await ReplyAsync(SiotrixConstants.BotSuccess);
                return;
            }

            await GuildEmbedFooter.SetGuildFooterText(val, txt);
            await ReplyAsync(SiotrixConstants.BotSuccess);
        }

        [Command("gthumbnail")]
        [Summary("Will list bots current thumbnail image link.")]
        [Remarks(" - no additional arguments needed.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildThumbNailAsync()
        {
            var val = await Context.GetGuildThumbNailAsync();
            await ReplyAsync(val.ThumbNail);
        }

        [Command("gthumbnail")]
        [Summary("Will set bots thumbnail image.")]
        [Remarks(
            "<url> - url of picture to assign as bot thumbnail **note** using keyword reset will reset to Siotrix image.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildThumbNailAsync(Uri url)
        {
            var val = await Context.GetGuildThumbNailAsync();

            if (url.ToString().Equals("reset"))
            {
                await GuildEmbedThumbnail.SetGuildThumbNail(val, SiotrixConstants.BotAvatar);
                await ReplyAsync(SiotrixConstants.BotSuccess);
                return;
            }

            await GuildEmbedThumbnail.SetGuildThumbNail(val, url.ToString());
            await ReplyAsync(SiotrixConstants.BotSuccess);
        }

        [Command("gwebsite")]
        [Alias("gweb")]
        [Summary("Will list bots current website.")]
        [Remarks(" - no additional arguments needed.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildWebSiteAsync()
        {
            var val = await Context.GetGuildUrlAsync();
            await ReplyAsync(val.SiteUrl);
        }

        [Command("gwebsite")]
        [Alias("gweb")]
        [Summary("Will set bots website.")]
        [Remarks(
            "<url> - url of bots website (guild website maybe?) **note** using keyword reset will reset to Siotrix website.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildWebSiteAsync(Uri url)
        {
            var val = await Context.GetGuildUrlAsync();

            if (url.ToString().Equals("reset"))
            {
                await GuildEmbedUrl.SetGuildUrl(val, SiotrixConstants.BotUrl);
                await ReplyAsync(SiotrixConstants.BotSuccess);
                return;
            }

            await GuildEmbedUrl.SetGuildUrl(val, url.ToString());
            await ReplyAsync(SiotrixConstants.BotSuccess);
        }

        [Command("gdescription")]
        [Alias("gdesc")]
        [Summary("Will list bots current description.")]
        [Remarks(" - no additional arguments needed.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildDescriptionAsync()
        {
            var val = await Context.GetGuildDescriptionAsync();

            if (val.Description == null)
            {
                await ReplyAsync($"No description set for {Context.Guild.Name}");
                return;
            }
            await ReplyAsync(val.Description);
        }

        [Command("gdescription")]
        [Alias("gdesc")]
        [Summary("Will set bots description for your guild.")]
        [Remarks("<text> - text you would like to use as a guild description.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildDescriptionAsync([Remainder] string desc)
        {
            var val = await Context.GetGuildDescriptionAsync();

            if (desc.Trim().Equals("reset"))
            {
                await GuildEmbedDescription.SetGuildDescription(val, "");
                await ReplyAsync(SiotrixConstants.BotSuccess);
                return;
            }

            await GuildEmbedDescription.SetGuildDescription(val, desc);
            await ReplyAsync(SiotrixConstants.BotSuccess);
        }


        [Command("color")]
        [Summary("Set or list your guilds official embed color.")]
        [Remarks(" - no additional arguments needed.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildColorAsync()
        {
            var regexColorCode = new Regex("^#[A-Fa-f0-9]{6}$");
            var regexRgbCode =
                new Regex(
                    "^\\s*(0|[1-9]\\d?|1\\d\\d?|2[0-4]\\d|25[0-5])\\s*,\\s*(0|[1-9]\\d?|1\\d\\d?|2[0-4]\\d|25[0-5])\\s*,\\s*(0|[1-9]\\d?|1\\d\\d?|2[0-4]\\d|25[0-5])\\s*$");
            var currentGColor = await Context.GetGuildColorAsync();

            var preColor = currentGColor.ColorHex;

            if (preColor == "0x000000")
                preColor = "0x010101";
            var currentModifiedGColor = new Color(Convert.ToUInt32(preColor, 16));
            var currentHexColor = currentModifiedGColor.ToString();

            if (currentHexColor.Length != 7)
            {
                currentHexColor = currentHexColor.Substring(1);
                currentHexColor = "#" + currentHexColor.PadLeft(6, '0');
            }

            await ReplyAsync(
                $"Give me any value of color (Hex, RGB, or a name) to set your guild color (Example Hex: #FF43A4).\nYour Current Guild Hex Code is: {Format.Bold(currentHexColor)}. Type list for a breakdown of your current color.");
            var response = await _interactive.WaitForMessage(Context.User, Context.Channel, TimeSpan.FromSeconds(30));

            if (response.Content == "cancel")
            {
                await ReplyAsync("I have cancelled your request. You will keep your current color.");
                return;
            }
            _timer.Start();

            if (response.Content == "list")
            {
                var cleanHex = currentHexColor.Replace("#", "0x").ToLower(); //strip # and add 0x for dictionary search
                var colornamelower = HexColorDict.ColorName(cleanHex) ?? "no name found"; //look up hex in dictionary


                var text = new CultureInfo("en-US").TextInfo;
                var colorname = text.ToTitleCase(colornamelower);

                var rgbvalue = HextoRgb.HexadecimalToRgb(currentHexColor); // convert hex to an RGB value
                var red = Convert.ToString(rgbvalue.R); // Red Property
                var green = Convert.ToString(rgbvalue.G); // Green property
                var blue = Convert.ToString(rgbvalue.B); // Blue property  

                if (colorname == "Black")
                {
                    colorname = "Black (For Discord)";
                    currentHexColor = "#010101";
                    rgbvalue.R = 1;
                    rgbvalue.G = 1;
                    rgbvalue.B = 1;
                }

                var hexcolor = currentHexColor.ToUpper();

                var embed = await GetEmbed(hexcolor, colorname, hexcolor, rgbvalue);
                await ReplyAsync("", embed: embed);

                return;
            }

            var colorchoice = response.Content.ToLower();

            if (regexColorCode.IsMatch(colorchoice.Trim()))
            {
                if (colorchoice.Trim() == "#000000")
                    colorchoice = "#010101";

                var cleanHex = colorchoice.Replace("#", "0x").ToLower(); //strip # and add 0x for dictionary search

                var colornamelower = HexColorDict.ColorName(cleanHex) ?? "no name found"; //look up hex in dictionary


                await GuildEmbedColorExtensions.SetGuildColor(currentGColor, cleanHex);

                var text = new CultureInfo("en-US").TextInfo;
                var colorname = text.ToTitleCase(colornamelower);

                var hexcolor = colorchoice.ToUpper();

                var rgbvalue = HextoRgb.HexadecimalToRgb(colorchoice); // convert hex to an RGB value
                var red = Convert.ToString(rgbvalue.R); // Red Property
                var green = Convert.ToString(rgbvalue.G); // Green property
                var blue = Convert.ToString(rgbvalue.B); // Blue property  

                if (colorname == "Black")
                {
                    colorname = "Black (For Discord)";
                    hexcolor = "#010101";
                    rgbvalue.R = 1;
                    rgbvalue.G = 1;
                    rgbvalue.B = 1;
                }

                var embed = await GetEmbed(hexcolor, colorname, hexcolor, rgbvalue);
                await ReplyAsync("", embed: embed);
            }
            else if (HexColorDict.colorHex.ContainsKey(colorchoice))
            {
                if (colorchoice.ToLower() == "black")
                    colorchoice = "black (for Discord)";

                var colorHex = HexColorDict.ColorHex(colorchoice); // look up hex in color name Dictionary

                await GuildEmbedColorExtensions.SetGuildColor(currentGColor, colorHex);

                var cleanhex = colorHex.Replace("0x", "#").ToUpper(); // convert the hex back to #FFFFFF format

                var text = new CultureInfo("en-US").TextInfo;
                var colorname = text.ToTitleCase(colorchoice);

                var rgbvalue = HextoRgb.HexadecimalToRgb(cleanhex); // convert hex to RGB value

                if (colorname == "Black")
                {
                    colorname = "Black (For Discord)";
                    cleanhex = "#010101";
                    rgbvalue.R = 1;
                    rgbvalue.G = 1;
                    rgbvalue.B = 1;
                }

                var embed = await GetEmbed(colorname, colorname, cleanhex, rgbvalue);
                await ReplyAsync("", embed: embed);
            }
            else if (regexRgbCode.IsMatch(colorchoice))
            {
                var str = colorchoice.Split(new[] {',', ' '},
                    StringSplitOptions.RemoveEmptyEntries); // Split up string of numbers into Red/Green/Blue
                var red = int.Parse(str[0]); //convert to red int
                var green = int.Parse(str[1]); //convert to green int
                var blue = int.Parse(str[2]); // convery to blue int
                var data = new RgBtoHex.Rgb((byte) red, (byte) green,
                    (byte) blue); // convert broken out ints into a data struct - prep for conversion

                var colorhex = RgBtoHex.RgbToHexadecimal(data); // convert RGB input into hex      

                if (colorhex == "#000000")
                    colorhex = "#010101";

                if (!regexColorCode.IsMatch(colorhex))
                {
                    var colorname = "No Name Found";
                    var hexcolorcaps = "Hex Not Available";
                    var r = (byte) red;
                    var g = (byte) green;
                    var b = (byte) blue;
                    var rgbvalue = new HextoRgb.Rgb(r, g, b);
                    var embed = await GetEmbed(colorchoice, colorname, hexcolorcaps, rgbvalue);
                    await ReplyAsync("", embed: embed);
                }
                else
                {
                    var cleanHex = colorhex.Replace("#", "0x"); // replace # with 0x for dictionary lookup
                    var hexcolorcaps = colorhex.ToUpper();

                    var cleanHexLower = cleanHex.ToLower();

                    var colornamelower = HexColorDict.ColorName(cleanHexLower); // get color name

                    await GuildEmbedColorExtensions.SetGuildColor(currentGColor, cleanHexLower);


                    if (colornamelower == null)
                        colornamelower = "no name found";

                    var text = new CultureInfo("en-US").TextInfo;
                    var colorname = text.ToTitleCase(colornamelower);

                    var rgbvalue = HextoRgb.HexadecimalToRgb(colorhex);

                    if (colorname == "Black")
                    {
                        colorname = "Black (For Discord)";
                        hexcolorcaps = "#010101";
                        rgbvalue.R = 1;
                        rgbvalue.G = 1;
                        rgbvalue.B = 1;
                    }

                    var embed = await GetEmbed(colorchoice, colorname, hexcolorcaps, rgbvalue);
                    await ReplyAsync("", embed: embed);
                }
            }
            else
            {
                await ReplyAsync("You did not input a correct hex, color name, or RGB value.");
            }
        }

        private async Task<EmbedBuilder> GetEmbed(string colorchoice, string colorname, string colorhex,
            HextoRgb.Rgb rgbvalue)
        {
            _timer.Stop();
            var gIconUrl = await Context.GetGuildIconUrlAsync();
            var gName = await Context.GetGuildNameAsync();
            var gUrl = await Context.GetGuildUrlAsync();
            var gThumbnail = await Context.GetGuildThumbNailAsync();
            var gFooter = await Context.GetGuildFooterAsync();
            var gPrefix = await Context.GetGuildPrefixAsync();
            var red = Convert.ToString(rgbvalue.R); // Red Property
            var green = Convert.ToString(rgbvalue.G); // Green property
            var blue = Convert.ToString(rgbvalue.B); // Blue property
            var color = new Color(rgbvalue.R, rgbvalue.G, rgbvalue.B);

            if (colorname == null)
                colorname = "No Name Found";

            if (colorchoice == null)
                colorchoice = "Colorchoice is empty";

            if (colorhex == null)
                colorhex = "No hex equivalent found.";

            var builder = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(gIconUrl.Avatar)
                    .WithName(gName.GuildName)
                    .WithUrl(gUrl.SiteUrl))
                .WithThumbnailUrl(gThumbnail.ThumbNail)
                .WithFooter(new EmbedFooterBuilder()
                    .WithIconUrl(gFooter.FooterIcon)
                    .WithText(gFooter.FooterText))
                .WithTimestamp(DateTime.UtcNow);
            builder.Title = $"Color Embed Information for: {colorchoice}";
            builder.Color = color;
            builder.AddField(x =>
            {
                x.IsInline = true;
                x.Name = "Color Name: ";
                x.Value = Format.Bold(colorname);
            });

            builder.AddField(x =>
            {
                x.IsInline = true;
                x.Name = "Hex Color Code: ";
                x.Value = Format.Bold(colorhex);
            });

            builder.AddField(x =>
            {
                x.IsInline = true;
                x.Name = "RGB Value: ";
                x.Value = $"RED:{Format.Bold(red)} GREEN:{Format.Bold(green)} BLUE:{Format.Bold(blue)}";
            });
            builder.WithDescription($"In {_timer.ElapsedMilliseconds}ms");

            return builder;
        }

        [Command("gname")]
        [Summary("Lists your guilds current name thats been set for embeds.")]
        [Remarks(" - no additional arguments needed.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildNameAsync()
        {
            var val = await Context.GetGuildNameAsync();
            await ReplyAsync(val.GuildName);
        }

        [Command("gname")]
        [Summary("Sets guild name.")]
        [Remarks(
            "<name> - Name of guild you want to use in embeds. **note** reset will reset to your actual guild name.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildNameAsync([Remainder] string txt)
        {
            var val = await Context.GetGuildNameAsync();

            if (txt.Equals("reset"))
            {
                await GuildEmbedName.SetGuildName(val, Context.Guild.Name);
                await ReplyAsync(SiotrixConstants.BotSuccess);
                return;
            }

            await GuildEmbedName.SetGuildName(val, txt);
            await ReplyAsync(SiotrixConstants.BotSuccess);
        }

        [Command("gavatar")]
        [Summary("Will list bots current guild avatar for embeds.")]
        [Remarks(" - no additional arguments needed.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildAvatarAsync()
        {
            var val = await Context.GetGuildIconUrlAsync();
            await ReplyAsync(val.Avatar);
        }

        [Command("gavatar")]
        [Summary("Will set bots guild avatar for embeds.")]
        [Remarks(
            "<url> - url of picture to assign as bot avatar **note** using keyword reset will reset to Siotrix embed avatar.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildAvatarAsync(Uri url)
        {
            var val = await Context.GetGuildIconUrlAsync();

            if (url.ToString().Equals("reset"))
            {
                await GuildEmbedIconUrl.SetGuildAvatar(val, SiotrixConstants.BotAvatar);
                await ReplyAsync(SiotrixConstants.BotSuccess);
                return;
            }

            await GuildEmbedIconUrl.SetGuildAvatar(val, url.ToString());
            await ReplyAsync(SiotrixConstants.BotSuccess);
        }

        [Command("prefix")]
        [Summary("Will list bots current guild prefix.")]
        [Remarks(" - no additional arguments needed.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task PrefixAsync()
        {
            var val = await Context.GetGuildPrefixAsync();
            await ReplyAsync(val.Prefix);
        }

        [Command("prefix")]
        [Summary("Will set bot prefix for your guild.")]
        [Remarks(
            "<prefix> - Any prefix you'd like, up to 10 characters for your guild. **note** reset will change prefix to !.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task PrefixAsync([Remainder] string txt)
        {
            //TODO: Prefix needs more work
            // string str = CheckEmojiText(txt);
            var val = await Context.GetGuildPrefixAsync();

            if (txt.Equals("reset"))
            {
                await PrefixExtensions.SetGuildPrefix(val, SiotrixConstants.BotPrefix);
                await ReplyAsync(SiotrixConstants.BotSuccess);
                return;
            }

            await PrefixExtensions.SetGuildPrefix(val, txt);
            await ReplyAsync(SiotrixConstants.BotSuccess);
        }

        private bool CheckString(string str)
        {
            var isFounded = false;
            string[] spec = {"!", "~", "#", "$", "%", "&", "*", ":", "/", "|", "+", "=", "-", "_", ">", "<", "reset"};
            foreach (var element in spec)
                if (str.Equals(element))
                {
                    isFounded = true;
                    break;
                }
            return isFounded;
        }

        public static string FromText(string text)
        {
            text = text.Trim(':');

            var unicode = default(string);
            if (EmojiMap.Map.TryGetValue(text, out unicode))
                return unicode;
            throw new ArgumentException("The given alias could not be matched to a Unicode Emoji.", nameof(text));
        }

        public string CheckEmojiText(string str)
        {
            var newStr = str.Trim(':');
            if (newStr.Equals(str))
            {
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>{0}", newStr);
                return newStr;
            }
            var unicode = default(string);
            if (EmojiMap.Map.TryGetValue(newStr, out unicode))
                Console.WriteLine("==========================={0}", unicode);
            return unicode;
        }

        [Command("gmotd")]
        [Summary("Lists guild current motd.")]
        [Remarks(" - no additional arguments needed.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildMotdAsync()
        {
            var val = await Context.GetGuildMotdAsync();

            if (val.Message == null)
            {
                await ReplyAsync($"No MOTD set for {Context.Guild.Name}");
                return;
            }
            await ReplyAsync(val.Message);
        }

        [Command("gmotd")]
        [Summary("Sets guild motd.")]
        [Remarks("<motd> - Motd text for guild. **note** reset will revert back to default motd.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildMotdAsync([Remainder] string str)
        {
            var val = await Context.GetGuildMotdAsync();

            if (str.Trim().Equals("reset"))
            {
                await GuildMotdExtensions.SetGuildMotd(val, "");
                await ReplyAsync(SiotrixConstants.BotSuccess);
                return;
            }

            await GuildMotdExtensions.SetGuildMotd(val, str);
            await ReplyAsync(SiotrixConstants.BotSuccess);
        }

        [Command("autodelete")]
        [Summary("")]
        [Remarks("")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task GuildAutoDeleteAsync(string user = "None")
        {
            var guildId = Context.Guild.Id;
            var option = 0;
            string status = null;
            using (var db = new LogDatabase())
            {
                var val = new DiscordGuildAutoDelete();
                switch (user)
                {
                    case "EveryOne":
                        option = 1;
                        break;
                    case "Moderator":
                        option = 2;
                        break;
                    case "User":
                        option = 3;
                        break;
                    default:
                        option = 0;
                        break;
                }
                try
                {
                    var result = db.Gautodeletes.Where(x => x.GuildId == guildId.ToLong() && x.Option == option);
                    if (result.Any())
                    {
                        db.Gautodeletes.RemoveRange(result);
                        status = "✖️ : Auto-delete command function has been deleted.";
                    }
                    else
                    {
                        var record = new DiscordGuildAutoDelete();
                        record.Option = option;
                        record.GuildId = guildId.ToLong();
                        db.Gautodeletes.Add(record);
                        status = "✅ : Auto-delete command function has been applied.";
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            await ReplyAsync(status);
        }

        [Command("nickname")]
        [Summary("Lists Siotrix's nickname.")]
        [Remarks(" - no additional arguments needed.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task NicknameAsync()
        {
            await ReplyAsync(Context.Guild.CurrentUser.Nickname ?? Context.Guild.CurrentUser.ToString());
        }

        [Command("nickname")]
        [Summary("Sets Siotrix's nickname.")]
        [Remarks(
            "<name> - Set a nickname for Siotrix just for your guild. **note** reset will change it back to Siotrx.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task NicknameAsync([Remainder] string name)
        {
            var self = Context.Guild.CurrentUser;
            if (name.Equals("reset"))
                name = SiotrixConstants.BotName;
            await self.ModifyAsync(x => { x.Nickname = name; });
            await ReplyAsync(SiotrixConstants.BotSuccess);
        }

        [Command("warnsetup")]
        [Summary("Will do an interactive setup for automated warning.")]
        [Remarks(" - no additional arguments needed.")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task WarnSetupAsync()
        {
            var val = await WarningExtensions.GetWarnSettingsAsync(Context.Guild.Id);

            if (val == null)
            {
                await WarningExtensions.CreateWarnSettingsAsync(Context.Guild.Id, SiotrixConstants.TimesBeforeMute,
                    SiotrixConstants.MuteTimeLengthMinutes, SiotrixConstants.TimesBeforeBan,
                    SiotrixConstants.BanTimeLengthMinutes, SiotrixConstants.SrsInfractionsBeforePermBan,
                    SiotrixConstants.WarningFalloffMinutes);
                val = await WarningExtensions.GetWarnSettingsAsync(Context.Guild.Id);
                var muteTimeLength = TimeSpan.FromMinutes(val.MuteTimeLengthMinutes);
                var banTimeLength = TimeSpan.FromMinutes(val.BanTimeLengthMinutes);
                var falloff = TimeSpan.FromMinutes(val.WarningFalloffMinutes);

                await ReplyAsync(
                    $"Welcome to the initial setup of my automated warning system! Please respond within 30 seconds for each question." +
                    " if you type cancel at any time, it will close the process and you will have to start over. Since this is your first time i've" +
                    "set you up with default settings as follows:\n" +
                    $"Warning points before user muted: {val.TimesBeforeMute}.\n" +
                    $"Length of mute time a user will receive once they break the warning threshold: {val.MuteTimeLengthMinutes} minutes ({muteTimeLength.ToTimespanPrettyFormat()})\n" +
                    $"Warning points before user is banned: {val.TimesBeforeBan}.\n" +
                    $"Length of ban time a user will receive once they break the warning threshold: {val.BanTimeLengthMinutes} minutes ({banTimeLength.ToTimespanPrettyFormat()})\n" +
                    $"Number of serious infractions (times they have been temp banned) before they receive a permenant ban {val.SrsInfractionsBeforePermBan}.\n" +
                    $"Falloff timer for each warning point a user has: {val.WarningFalloffMinutes} minutes ({falloff.ToTimespanPrettyFormat()}).\n\n" +
                    "Please enter the number of warning points before a user should get muted(you can set the length of mute afterwards):");
            }
            else
            {
                var muteTimeLength = TimeSpan.FromMinutes(val.MuteTimeLengthMinutes);
                var banTimeLength = TimeSpan.FromMinutes(val.BanTimeLengthMinutes);
                var falloff = TimeSpan.FromMinutes(val.WarningFalloffMinutes);

                await ReplyAsync(
                    $"Welcome to the automated warning sytem setup. I've retreived your current settings for your reference:\n" +
                    $"Warning points before user muted: {val.TimesBeforeMute}.\n" +
                    $"Length of mute time a user will receive once they break the warning threshold: {val.MuteTimeLengthMinutes} minutes ({muteTimeLength.ToTimespanPrettyFormat()})\n" +
                    $"Warning points before user is banned: {val.TimesBeforeBan}.\n" +
                    $"Length of ban time a user will receive once they break the warning threshold: {val.BanTimeLengthMinutes} minutes ({banTimeLength.ToTimespanPrettyFormat()})\n" +
                    $"Number of serious infractions (times they have been temp banned) before they receive a permenant ban {val.SrsInfractionsBeforePermBan}.\n" +
                    $"Falloff timer for each warning point a user has: {val.WarningFalloffMinutes} minutes ({falloff.ToTimespanPrettyFormat()}).\n\n" +
                    "Please enter the number of warning points before a user should get muted(you can set the length of mute afterwards) (value 1 to 99): ");
            }



            var timesBeforeMute =
                await _interactive.WaitForMessage(Context.User, Context.Channel, TimeSpan.FromSeconds(30));

            if (timesBeforeMute.Content == "cancel")
            {
                await ReplyAsync("I have cancelled your request. You will keep your current settings.");
                return;
            }

            if (timesBeforeMute.Content.Length > 0 && timesBeforeMute.Content.Length < 3 &&
                timesBeforeMute.Content.All(c => c >= '0' && c <= '9') && timesBeforeMute.Content != "0" &&
                timesBeforeMute.Content != "00")
            {
                int x;
                int.TryParse(timesBeforeMute.Content, out x);
                await WarningExtensions.ModifyWarnTimesBeforeMute(val, x);
            }
            else
            {
                await ReplyAsync(
                    $"{timesBeforeMute.Content} is not a number 1 to 99. I will keep your current setting of **{val.TimesBeforeMute}**.");
            }

            await Task.Delay(1000);

            await ReplyAsync(
                "Please enter the length of mute time a user will receive once they break the warning threshold (for example: 3 days 12 hours 45 minutes):");
            var muteTimeLengthMinutes =
                await _interactive.WaitForMessage(Context.User, Context.Channel, TimeSpan.FromSeconds(30));

            if (muteTimeLengthMinutes.Content == "cancel")
            {
                await ReplyAsync("I have cancelled your request. You will keep your current settings.");
                return;
            }

            try
            {
                TimeSpan muteTimeSpan = TimeSpan.Parse(muteTimeLengthMinutes.Content);
                if (muteTimeSpan != null)
                {
                    long muteTimeMinutes = (long) muteTimeSpan.TotalMinutes;
                    await WarningExtensions.ModifyWarnMuteTimeLengthMinutes(val, muteTimeMinutes);
                    await ReplyAsync(
                        $"I have updated to the mute time of **{muteTimeMinutes} minutes ({muteTimeSpan.ToTimespanPrettyFormat()})**.");
                }
                else
                {
                    var muteTimeLength = TimeSpan.FromMinutes(val.MuteTimeLengthMinutes);
                    await ReplyAsync(
                        $"{muteTimeLengthMinutes.Content} is not a valid time frame. I will keep your current setting of **{val.MuteTimeLengthMinutes} minutes ({muteTimeLength.ToTimespanPrettyFormat()})**.");
                }
            }
            catch (Exception)
            {
                var muteTimeLength = TimeSpan.FromMinutes(val.MuteTimeLengthMinutes);
                await ReplyAsync(
                    $"{muteTimeLengthMinutes.Content} is not a valid time frame. I will keep your current setting of **{val.MuteTimeLengthMinutes} minutes ({muteTimeLength.ToTimespanPrettyFormat()})**.");
            }

            await Task.Delay(1000);

            await ReplyAsync(
                "Please enter the number of warning points before a user should get banned(you can set the length of ban afterwards) (value 1 to 99): ");

            var timesBeforeBan =
                await _interactive.WaitForMessage(Context.User, Context.Channel, TimeSpan.FromSeconds(30));

            if (timesBeforeBan.Content == "cancel")
            {
                await ReplyAsync("I have cancelled your request. You will keep your current settings.");
                return;
            }

            if (timesBeforeBan.Content.Length > 0 && timesBeforeBan.Content.Length < 3 &&
                timesBeforeBan.Content.All(c => c >= '0' && c <= '9') && timesBeforeBan.Content != "0" &&
                timesBeforeBan.Content != "00")
            {
                int x;
                int.TryParse(timesBeforeBan.Content, out x);
                await WarningExtensions.ModifyWarnTimesBeforeBan(val, x);
            }
            else
            {
                await ReplyAsync(
                    $"{timesBeforeBan.Content} is not a number 1 to 99. I will keep your current setting of **{val.TimesBeforeBan}**.");
            }

            await Task.Delay(1000);

            await ReplyAsync(
                "Please enter the length of ban time a user will receive once they break the warning threshold (for example: 3 days 12 hours 45 minutes):");
            var banTimeLengthMinutes =
                await _interactive.WaitForMessage(Context.User, Context.Channel, TimeSpan.FromSeconds(30));

            if (banTimeLengthMinutes.Content == "cancel")
            {
                await ReplyAsync("I have cancelled your request. You will keep your current settings.");
                return;
            }

            try
            {
                TimeSpan banTimeSpan = TimeSpan.Parse(banTimeLengthMinutes.Content);
                if (banTimeSpan != null)
                {
                    long banTimeMinutes = (long) banTimeSpan.TotalMinutes;
                    await WarningExtensions.ModifyWarnMuteTimeLengthMinutes(val, banTimeMinutes);
                    await ReplyAsync(
                        $"I have updated to the ban time of **{banTimeMinutes} minutes ({banTimeSpan.ToTimespanPrettyFormat()})**.");
                }
                else
                {
                    var banTimeLength = TimeSpan.FromMinutes(val.BanTimeLengthMinutes);
                    await ReplyAsync(
                        $"{banTimeLengthMinutes.Content} is not a valid time frame. I will keep your current setting of **{val.BanTimeLengthMinutes} minutes ({banTimeLength.ToTimespanPrettyFormat()})**.");
                }
            }
            catch (Exception)
            {
                var banTimeLength = TimeSpan.FromMinutes(val.BanTimeLengthMinutes);
                await ReplyAsync(
                    $"{banTimeLengthMinutes.Content} is not a valid time frame. I will keep your current setting of **{val.BanTimeLengthMinutes} minutes ({banTimeLength.ToTimespanPrettyFormat()})**.");
            }

            await Task.Delay(1000);

            await ReplyAsync(
                "Please enter the number of serious offenses(1 point per temporary ban either automatic or manual) before a permenant ban (value 1 to 99): ");

            var srsInfractionsBeforePermBan =
                await _interactive.WaitForMessage(Context.User, Context.Channel, TimeSpan.FromSeconds(30));

            if (srsInfractionsBeforePermBan.Content == "cancel")
            {
                await ReplyAsync("I have cancelled your request. You will keep your current settings.");
                return;
            }

            if (srsInfractionsBeforePermBan.Content.Length > 0 && srsInfractionsBeforePermBan.Content.Length < 3 &&
                srsInfractionsBeforePermBan.Content.All(c => c >= '0' && c <= '9') &&
                srsInfractionsBeforePermBan.Content != "0" && srsInfractionsBeforePermBan.Content != "00")
            {
                int x;
                int.TryParse(srsInfractionsBeforePermBan.Content, out x);
                await WarningExtensions.ModifyWarnSrsInfractionsBeforePermBan(val, x);
            }
            else
            {
                await ReplyAsync(
                    $"{srsInfractionsBeforePermBan.Content} is not a number 1 to 99. I will keep your current setting of **{val.SrsInfractionsBeforePermBan}**.");
            }

            await Task.Delay(1000);

            await ReplyAsync(
                "Please enter the length of falloff time on each warning point. Once falloff time is reached, users with a warning point will be reduced by 1 warning." +
                "This allows users to automatically be forgiven over time (for example: 3 days 12 hours 45 minutes):");
            var warningFalloffMinutes =
                await _interactive.WaitForMessage(Context.User, Context.Channel, TimeSpan.FromSeconds(30));

            if (warningFalloffMinutes.Content == "cancel")
            {
                await ReplyAsync("I have cancelled your request. You will keep your current settings.");
                return;
            }

            try
            {
                TimeSpan falloffTimeSpan = TimeSpan.Parse(warningFalloffMinutes.Content);
                if (falloffTimeSpan != null)
                {
                    long falloffTimeMinutes = (long) falloffTimeSpan.TotalMinutes;
                    await WarningExtensions.ModifyWarnMuteTimeLengthMinutes(val, falloffTimeMinutes);
                    await ReplyAsync(
                        $"I have updated to the falloff time of **{falloffTimeMinutes} minutes ({falloffTimeSpan.ToTimespanPrettyFormat()})**.");
                }
                else
                {
                    var falloffTimeLength = TimeSpan.FromMinutes(val.WarningFalloffMinutes);
                    await ReplyAsync(
                        $"{warningFalloffMinutes.Content} is not a valid time frame. I will keep your current setting of **{val.WarningFalloffMinutes} minutes ({falloffTimeLength.ToTimespanPrettyFormat()})**.");
                }
            }
            catch (Exception)
            {
                var falloffTimeLength = TimeSpan.FromMinutes(val.WarningFalloffMinutes);
                await ReplyAsync(
                    $"{warningFalloffMinutes.Content} is not a valid time frame. I will keep your current setting of **{val.WarningFalloffMinutes} minutes ({falloffTimeLength.ToTimespanPrettyFormat()})**.");
            }

            var finalVal = await WarningExtensions.GetWarnSettingsAsync(Context.Guild.Id);

            await Task.Delay(1000);

            await WarningExtensions.ModifyWarnIsActive(finalVal, true);

            var finalMuteTimeLength = TimeSpan.FromMinutes(finalVal.MuteTimeLengthMinutes);
            var finalBanTimeLength = TimeSpan.FromMinutes(finalVal.BanTimeLengthMinutes);
            var finalFalloff = TimeSpan.FromMinutes(finalVal.WarningFalloffMinutes);
            var prefix = await Context.GetGuildPrefixAsync();

            await ReplyAsync(
                $"You are all set up, and I have actrivated the automated system! Here are the final settings for your reference:\n" +
                $"Warning points before user muted: {finalVal.TimesBeforeMute}.\n" +
                $"Length of mute time a user will receive once they break the warning threshold: {finalVal.MuteTimeLengthMinutes} minutes ({finalMuteTimeLength.ToTimespanPrettyFormat()})\n" +
                $"Warning points before user is banned: {finalVal.TimesBeforeBan}.\n" +
                $"Length of ban time a user will receive once they break the warning threshold: {finalVal.BanTimeLengthMinutes} minutes ({finalBanTimeLength.ToTimespanPrettyFormat()})\n" +
                $"Number of serious infractions (times they have been temp banned) before they receive a permenant ban {finalVal.SrsInfractionsBeforePermBan}.\n" +
                $"Falloff timer for each warning point a user has: {finalVal.WarningFalloffMinutes} minutes ({finalFalloff.ToTimespanPrettyFormat()}).\n\n" +
                $"**NOTE** you can deactivate/re-activate the system at anytime using {prefix.Prefix}settings warntoggle");
        }

        [Command("warntoggle")]
        [Summary("Will toggle on or off the automated warning module.")]
        [Remarks("- No other argument needed")]
        [MinPermissions(AccessLevel.GuildAdmin)]
        public async Task WarnToggleAsync()
        {
            var val = await WarningExtensions.GetWarnSettingsAsync(Context.Guild.Id);

            if (val == null)
            {
                await WarningExtensions.CreateWarnSettingsAsync(Context.Guild.Id, SiotrixConstants.TimesBeforeMute,
                    SiotrixConstants.MuteTimeLengthMinutes, SiotrixConstants.TimesBeforeBan,
                    SiotrixConstants.BanTimeLengthMinutes, SiotrixConstants.SrsInfractionsBeforePermBan,
                    SiotrixConstants.WarningFalloffMinutes);
                val = await WarningExtensions.GetWarnSettingsAsync(Context.Guild.Id);                
            }

            var isActive = val.IsActive;

            if (isActive)
            {
                await WarningExtensions.ModifyWarnIsActive(val, false);
                await ReplyAsync("Automated warnings have been disabled for the guild.");
            }
            else
            {
                await WarningExtensions.ModifyWarnIsActive(val, true);
                await ReplyAsync("Automated warnings have been disabled for the guild.");
            }
        }
    }
}