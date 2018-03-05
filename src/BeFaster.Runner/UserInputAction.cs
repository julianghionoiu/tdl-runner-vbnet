using System;
using TDL.Client.Runner;

namespace BeFaster.Runner
{
    public class UserInputAction : IActionProvider
    {
        private string[] args;

        public UserInputAction(string[] args)
        {
            this.args = args;
        }

        public string Get() =>
            args.Length > 0
                ? args[0]
                : Console.In.ReadLine();
    }
}
