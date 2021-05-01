using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Commands.F1
{
    public interface IF1Parser
    {
        string Name { get; }
        string Execute(SocialRatingUser user, IEnumerable<string> args);
    }
}
