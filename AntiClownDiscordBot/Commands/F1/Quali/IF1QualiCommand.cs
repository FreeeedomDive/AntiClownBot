using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiClownBot.Commands.F1.Quali
{
    public interface IF1QualiCommand
    {
        string Name { get; }
        string Execute(SocialRatingUser user, List<string> args);
    }
}
