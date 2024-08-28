using System;
using TDL.Client.Runner;

namespace BeFaster.Runner
{
    public class UserInputAction(string[] args) : IActionProvider
    {
        private readonly string[] args = args;

        public string Get() =>
            args.Length > 0
                ? args[0]
                : Console.In.ReadLine() ?? string.Empty;
    }
}
