using System.Collections.Generic;
using System.Linq;
using AntiClownBot.Commands.F1;
using AntiClownBot.Commands.Lottery;
using AntiClownBot.Commands.OtherCommands;
using AntiClownBot.Commands.RandomSelectCommands;
using AntiClownBot.Commands.SocialRatingCommands;
using AntiClownBot.Commands.StatsCommands;
using DSharpPlus;
using DSharpPlus.EventArgs;
using AntiClownBot.Commands.Lohotron;

namespace AntiClownBot.Commands
{
    public class CommandsManager
    {
        private readonly Dictionary<string, BaseCommand> _commands;
        public CommandsManager(DiscordClient client, Configuration config)
        {
            _commands = new Dictionary<string, BaseCommand>();
            
            RegisterCommand("!tribute", new TributeCommand(client, config));
            RegisterCommand("!socialstatus", new SocialStatusCommand(client, config));
            RegisterCommand("!rating", new RatingCommand(client, config));
            RegisterCommand("!when", new WhenCommand(client, config));
            
            RegisterCommand("!mystats", new UserStatsCommand(client, config));
            RegisterCommand("!stats", new EmojiStatsCommand(client, config));
            RegisterCommand("!pidor", new PidorStatsCommand(client, config));

            RegisterCommand("!roll", new RollCommand(client, config));
            RegisterCommand("!select", new SelectCommand(client, config));
            
            RegisterCommand("!ip", new IpCommand(client, config));
            RegisterCommand("!help", new HelpCommand(client, config, this));
            RegisterCommand("!allcommands", new AllCommandsCommand(client, config, this));

            RegisterCommand("!f1", new F1CommandParser(client, config));
            RegisterCommand("!lottery", new LotteryCommand(client, config));

            RegisterCommand("!lohotron", new LohotronCommand(client, config));
            RegisterCommand("!use", new UseCommand(client, config));
        }

        private void RegisterCommand(string name, BaseCommand baseCommand)
        {
            _commands.Add(name, baseCommand);
        }

        public bool GetCommandByName(string name, out BaseCommand command)
        {
            if (_commands.ContainsKey(name))
            {
                command = _commands[name];
                return true;
            }

            command = null;
            return false;
        }

        public IEnumerable<string> AllCommands => _commands.Keys.OrderBy(key => key);

        public async void ExecuteCommand(string name, MessageCreateEventArgs e, SocialRatingUser user)
        {
            if (!GetCommandByName(name, out var command))
            {
                await e.Message.RespondAsync($"Нет команды с именем {name}");
                return;
            }
            command.Execute(e, user);
        }
    }
}