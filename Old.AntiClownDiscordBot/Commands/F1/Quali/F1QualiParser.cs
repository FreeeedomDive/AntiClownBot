﻿using AntiClownBot.Commands.F1.Quali;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Commands.F1
{
    public class F1QualiParser : IF1Parser
    {
        protected readonly Configuration Config;
        protected readonly DiscordClient DiscordClient;
        private readonly Dictionary<string, IF1QualiCommand> commands;
        public F1QualiParser(DiscordClient client, Configuration configuration)
        {
            Config = configuration;
            DiscordClient = client;
            commands = new List<IF1QualiCommand>
            {
                new F1QualiStartCommand(DiscordClient, Config)
            }.ToDictionary(x => x.Name);
        }
        public string Name => "quali";
        public string Parse(ulong userId, List<string> lines)
        {
            var args = lines[0].Split(' ').Skip(2).ToList();
            if (!args.Any())
            {
                return "Данный блок команд создан для организации удобных ставок на квалификацию F1\n" +
                    "Побеждают те гонщики, которые квалифицируются в P11-P15\n" +
                    "Коэффициенты ставятся автоматически";
            }
            if (!commands.TryGetValue(args.First(), out var command))
            {
                return "Чел, такой команды нет";
            }
            return command.Execute(userId, lines.Skip(1).ToList());
        }
    }
}