﻿using Discord;
using Discord.WebSocket;
using GynBot.Common.Types;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GynBot
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _commands;

        public async Task Start()
        {
            EnsureConfigExists();                            // Ensure the configuration file has been created.
                                                             // Create a new instance of DiscordSocketClient.
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose                  // Specify console verbose information level.
            });

            _client.Log += (l)                               // Register the console log event.
                => Task.Run(()
                => Console.WriteLine($"[{l.Severity}] {l.Source}: {l.Exception?.ToString() ?? l.Message}"));

            await _client.LoginAsync(TokenType.Bot, Configuration.Load().Token);
            await _client.StartAsync();

            _commands = new CommandHandler();               // Initialize the command handler service
            await _commands.Install(_client);

            await Task.Delay(-1);                            // Prevent the console window from closing.
        }

        public static void EnsureConfigExists()
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "data")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "data"));

            string loc = Path.Combine(AppContext.BaseDirectory, "data/configuration.json");

            if (!File.Exists(loc))                              // Check if the configuration file exists.
            {
                var config = new Configuration();               // Create a new configuration object.

                Console.WriteLine("The configuration file has been created at 'data\\configuration.json', " +
                              "please enter your information.");
                Console.Write("Token: ");

                config.Token = Console.ReadLine();              // Read the bot token from console.
                config.Save();                                  // Save the new configuration object to file.
            }
            Console.WriteLine("Configuration Loaded...");
        }
    }
}