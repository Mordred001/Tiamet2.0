using CiarDrekiOpenAILibrary.Chat;
using CiarDrekiOpenAILibrary.Images;
using CiarDrekiOpenAILibrary.OpenAI;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Input;
using Tiamet2._0.Commands;
using Tiamet2._0.Data;
using static System.Collections.Specialized.BitVector32;

namespace Tiamet2._0
{
    public class Tiamat
    {
        private readonly DiscordSocketClient socketClient;
        private readonly CommandService commandService;
        private readonly OpenAI openAI;
        private Conversation tiamatChat;
        private Timer helltidetimer;
        private Timer worldbosstimer;
        private Timer legionstimer;

        private readonly Config configFile = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

        public Tiamat() 
        {
            socketClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.All
            });

            commandService = new CommandService();
            Console.WriteLine("Please enter your OpenAI Key: ");
            var openAIkey = Console.ReadLine();
            if (string.IsNullOrEmpty(openAIkey))
                openAIkey = configFile.OpenAIKey;
            openAI = new OpenAI(openAIkey);
            tiamatChat = openAI.Chat.CreateConversation();
            tiamatChat.AppendSystemMessage("Your name is Tiamat, you are a PC Gamer, XBox Gamer, PlayStation Gamer, World famous D&D Dungeon Master, Movie and TV buff, and costumer. You CosPlay the 5 headed dragon Tiamat and can help anyone with any question related to the topics you know. You are pro LGBTQ+, love Bernie Sanders, and are pro-choice. You don't like the police and believe that the cops should be defunded and replaced with civil servants.");

        }

        public async Task RunAsync()
        {
            socketClient.Log += Log;
            Console.WriteLine("Please enter your Discord Token: ");
            var dToken = Console.ReadLine();
            if (string.IsNullOrEmpty(dToken))
                dToken = configFile.DiscordToken;

            await RegisterCommandsAsync();

            await socketClient.LoginAsync(TokenType.Bot, dToken);

            await socketClient.StartAsync();

            await Task.Delay(3000);

            helltidetimer = new Timer(async (_) => await Diablo4.CheckEvents(configFile.Helltide, socketClient), null, TimeSpan.Zero, TimeSpan.FromMinutes(60));
            worldbosstimer = new Timer(async (_) => await Diablo4.CheckEvents(configFile.WorldBoss, socketClient), null, TimeSpan.Zero, TimeSpan.FromMinutes(120));
            legionstimer = new Timer(async (_) => await Diablo4.CheckEvents(configFile.Legions, socketClient), null, TimeSpan.Zero, TimeSpan.FromMinutes(120));
            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            if(!File.Exists("Log.txt"))
                File.Create("Log.txt");
            Console.WriteLine(arg);
            File.AppendAllText("Log.txt", arg.ToString());
            return Task.CompletedTask;
        }

        private async Task RegisterCommandsAsync()
        {
            socketClient.MessageReceived += HandleCommandAsync;
            socketClient.ReactionAdded += HandleReactionAddedAsync;
            socketClient.ReactionRemoved += HandleReactionRemovedAsync;


            await commandService.AddModulesAsync(System.Reflection.Assembly.GetEntryAssembly(), null);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null)
                return;
            var context = new SocketCommandContext(socketClient, message);

            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var commands = context.Message.Content.Substring(1);
                var commandArgs = commands.Split(' ', 2);
                foreach(var command in commandArgs)
                {
                    switch(command.ToLower())
                    {
                        case "helltide":
                        case "hell":
                        case "hell-tide":
                            if (!await Diablo4.CheckEvents(configFile.Helltide, socketClient))
                            {
                                var botChannel = socketClient.GetChannel(1123379017656053861) as SocketTextChannel;
                                botChannel.SendMessageAsync($"No Hell-Tide currently happening.");
                            }
                            break;
                        case "worldboss":
                        case "world":
                        case "world-boss":
                            if (!await Diablo4.CheckEvents(configFile.WorldBoss, socketClient))
                            {
                                var botChannel = socketClient.GetChannel(1123379017656053861) as SocketTextChannel;
                                botChannel.SendMessageAsync($"No World-Boss currently Spawning.");
                            }
                            break;
                        case "legion":
                        case "legions":
                            if (!await Diablo4.CheckEvents(configFile.Legions, socketClient))
                            {
                                var botChannel = socketClient.GetChannel(1123379017656053861) as SocketTextChannel;
                                botChannel.SendMessageAsync($"No Gathering Of Legions currently happening.");
                            }
                            break;
                    }
                }
            }
            else if(message.HasStringPrefix("/create", ref argPos))
            {
                var user = message.Author as IGuildUser;
                string roleName = "Patreon Supporter";

                // Check if the user has the Patreon Supporter role
                bool hasRole = user.RoleIds.Any(roleId =>
                    context.Guild.GetRole(roleId)?.Name == roleName);

                if (hasRole)
                {
                    var imagePrompt = context.Message.Content.Substring(7);
                    var imageRequest = new ImageRequest()
                    {
                        NumOfImages = 4,
                        Size = ImageSize._512,
                        ResponseFormat = ImageResponseFormat.Url,
                        Prompt = imagePrompt
                    };
                    await context.Channel.SendMessageAsync("Certainly, let me create that for you.");
                    var image = await openAI.ImageGenerations.CreateImageAsync(imagePrompt);
                    var embed = new EmbedBuilder()
                        .WithAuthor(user)
                        .WithDescription(imagePrompt)
                        .WithCurrentTimestamp()
                        .WithImageUrl(image.Data[0].Url)
                        .Build();
                    await context.Channel.SendMessageAsync(embed: embed);
                }
                else
                {
                    await context.Channel.SendMessageAsync("I'm sorry, but because the cost to run an AI isn't cheap, this feature is only available to Patreon Supporters.\n" +
                                            "If you want to become a Patreon Supporter, go here: www.patreon.com\n" +
                                            "```If you are currently a Patreon Supporter but don't have the role, please @Moderator or @Admin.```");
                }

            }
            else if(message.HasMentionPrefix(socketClient.CurrentUser, ref argPos))
            {
                var user = message.Author as IGuildUser;
                string roleName = "Patreon Supporter";

                // Check if the user has the Patreon Supporter role
                bool hasRole = user.RoleIds.Any(roleId =>
                    context.Guild.GetRole(roleId)?.Name == roleName);

                if (hasRole)
                {
                    var prompt = context.Message.Content.Substring(22);
                    await context.Channel.SendMessageAsync($"Absolutely! But give me a moment while I write my reply to: \"{prompt}\"");
                    tiamatChat.AppendUserInput(prompt);
                    var response = await tiamatChat.GetResponseFromChatbotAsync();
                    foreach (var chunk in Kobolds.SplitIntoChunks(response))
                    {
                        await context.Channel.SendMessageAsync(chunk);
                    }
                }
                else
                {
                    await context.Channel.SendMessageAsync("I'm sorry, but because the cost to run an AI isn't cheap, this feature is only available to Patreon Supporters.\n" +
                        "If you want to become a Patreon Supporter, go here: www.patreon.com\n" +
                        "```If you are currently a Patreon Supporter but don't have the role, please @Moderator or @Admin.```");
                }
            }

        }

        private async Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
        {
            if (reaction.MessageId == configFile.RolesMessageId)
            {
                var channel = await cachedChannel.GetOrDownloadAsync();
                var user = reaction.User.Value;
                var guild = (channel as IGuildChannel)?.Guild;
                var guildUser = await guild.GetUserAsync(user.Id);

                // Check if the reaction is from a valid user and has a specific emoji
                if (user.Id != socketClient.CurrentUser.Id)
                {
                    IRole? role = null;
                    switch (reaction.Emote.Name)
                    {
                        case "desktop":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "PC Gamer");
                            break;
                        case "xbox":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "XBox Gamer");
                            break;
                        case "playstation":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "PlayStation Gamer");
                            break;
                        case "dnd":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "Table Top Gamer");
                            break;
                        case "cosplay":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "CosPlay Nerd");
                            break;
                        case "movie":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "Movie/TV Nerd");
                            break;
                        case "book":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "Comic Nerd");
                            break;
                        default:
                            break;

                    }
                    if (role != null)
                    {
                        await guildUser.AddRoleAsync(role);
                        var botChannel = socketClient.GetChannel(configFile.BotChannelId) as SocketTextChannel;
                        await botChannel.SendMessageAsync($"Assigned role {role.Name} to {guildUser.Username}.");
                    }
                }
            }
        }

        private async Task HandleReactionRemovedAsync(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
        {
            if (reaction.MessageId == configFile.RolesMessageId)
            {
                var channel = await cachedChannel.GetOrDownloadAsync();
                var user = reaction.User.Value;
                var guild = (channel as IGuildChannel)?.Guild;
                var guildUser = await guild.GetUserAsync(user.Id);

                // Check if the reaction is from a valid user and has a specific emoji
                if (user.Id != socketClient.CurrentUser.Id)
                {
                    IRole? role = null;
                    switch (reaction.Emote.Name)
                    {
                        case "desktop":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "PC Gamer");
                            break;
                        case "xbox":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "XBox Gamer");
                            break;
                        case "playstation":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "PlayStation Gamer");
                            break;
                        case "dnd":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "Table Top Gamer");
                            break;
                        case "cosplay":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "CosPlay Nerd");
                            break;
                        case "movie":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "Movie/TV Nerd");
                            break;
                        case "book":
                            role = guild.Roles.FirstOrDefault(x => x.Name == "Comic Nerd");
                            break;
                        default:
                            break;

                    }
                    if (role != null)
                    {
                        await guildUser.RemoveRoleAsync(role);
                        var botChannel = socketClient.GetChannel(configFile.BotChannelId) as SocketTextChannel;
                        await botChannel.SendMessageAsync($"Removed role {role.Name} to {guildUser.Username}.");
                    }
                }
            }
        }


    }
}
