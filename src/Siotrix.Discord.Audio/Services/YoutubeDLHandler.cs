﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Siotrix.Discord.Audio
{
    public class YoutubeDlHandler
    {
        private readonly string _appdir = AppContext.BaseDirectory;

        public Task EnsureUpdatedAsync()
        {
            if (!Directory.Exists("cache/audio"))
                Directory.CreateDirectory("cache/audio");

            return Task.CompletedTask;
        }

        public Task<string> DownloadAsync(ulong guildId, string url)
        {
            var id = new Random().Next(1000, 1000000);
            var pathpart = Path.Combine(_appdir, $"cache/audio/{guildId}");
            var filepart = $"{id}.mp3";

            if (!Directory.Exists(pathpart))
                Directory.CreateDirectory(pathpart);

            var path = Path.Combine(pathpart, filepart);

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(_appdir, "youtube-dl.exe"),
                Arguments = $"-x -q --write-info-json --audio-format mp3 -o \"{path}\" \"{url}\"",
                UseShellExecute = false
            });

            process.WaitForExit();
            return Task.FromResult(path);
        }
    }
}