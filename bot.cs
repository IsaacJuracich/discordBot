// Copy and Paste for Discord Bots

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
internal class bot {
        private static CommandService _commands = new CommandService();
        private static IServiceProvider _services;
        public static DiscordSocketClient d;
        public static List<SocketMessage> collection = new List<SocketMessage>();
        public static string token = "";
        public static string activity = "";
        public static string prefix = "";
        public static async Task start() {
            try {
                DiscordSocketConfig config = new DiscordSocketConfig {
                    MessageCacheSize = 100,
                    AlwaysDownloadUsers = true,
                };
                d = new DiscordSocketClient(config);
                await RegisterCommands();
                await d.LoginAsync(TokenType.Bot, token, true);
                await d.StartAsync();
                await d.SetGameAsync(activity, null, ActivityType.Playing);
                await Task.Delay(-1);
            }
            catch (Exception e) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.StackTrace);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        private static async Task RegisterCommands() {
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(d)
                .AddSingleton(_commands)
                .BuildServiceProvider();
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            d.MessageReceived += handler;
        }
        private static async Task handler(SocketMessage arg) {
            collection.Add(arg);
            Console.WriteLine(arg);
            var message = arg as SocketUserMessage;
            int argPos = 0;
            var context = new SocketCommandContext(d, message);
            if (message.HasStringPrefix(prefix, ref argPos) || message.HasMentionPrefix(d.CurrentUser, ref argPos)) {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess && result.ErrorReason.ToLower() != "unknown command.")
                    Console.WriteLine(result.ErrorReason);
            }
        }
    }
