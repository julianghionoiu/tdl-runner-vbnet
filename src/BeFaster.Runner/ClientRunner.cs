using System;
using System.Collections.Generic;
using System.Linq;
using BeFaster.Runner.Utils;
using TDL.Client;
using TDL.Client.Actions;
using TDL.Client.Audit;

namespace BeFaster.Runner
{
    public class ClientRunner
    {
        private readonly string username;
        private readonly Dictionary<string, Func<string[], object>> solutions;
        private string hostname;
        private RunnerAction defaultRunnerAction;

        public static ClientRunner ForUsername(string username)
        {
            return new ClientRunner(username);
        }

        private ClientRunner(string username)
        {
            this.username = username;
            solutions = new Dictionary<string, Func<string[], object>>();
        }

        public ClientRunner WithServerHostname(string hostname)
        {
            this.hostname = hostname;
            return this;
        }

        public ClientRunner WithActionIfNoArgs(RunnerAction defaultRunnerAction)
        {
            this.defaultRunnerAction = defaultRunnerAction;
            return this;
        }

        public ClientRunner WithSolutionFor(string methodName, Func<string[], object> solution)
        {
            solutions.Add(methodName, solution);
            return this;
        }

        public void Start(string[] args)
        {
            if (!IsRecordingSystemOk())
            {
                Console.WriteLine("Please run `record_screen_and_upload` before continuing.");
                return;
            }

            var runnerAction = ExtractActionFrom(args).OrElse(defaultRunnerAction);

            Console.WriteLine("Chosen action is: " + runnerAction.LongName);

            var client = TdlClient.Build()
                .SetHostname(hostname)
                .SetUniqueId(username)
                .SetAuditStream(new ConsoleAuditStream())
                .Create();

            var processingRules = new ProcessingRules();

            processingRules
                .On("display_description")
                .Call(p => RoundManagement.DisplayAndSaveDescription(p[0], p[1]))
                .Then(ClientActions.Publish);

            foreach (var solution in solutions)
            {
                processingRules
                    .On(solution.Key)
                    .Call(solution.Value)
                    .Then(runnerAction.ClientAction);
            }

            client.GoLiveWith(processingRules);

            RecordingSystem.NotifyEvent(RoundManagement.GetLastFetchedRound(), runnerAction.ShortName);

            Console.Write("\nPress any key to exit... ");
            Console.ReadLine();
        }

        private static Optional<RunnerAction> ExtractActionFrom(IEnumerable<string> args)
        {
            var actionName = args.FirstOrDefault() ?? string.Empty;
            var action = RunnerAction.AllActions.FirstOrDefault(a => a.LongName.Equals(actionName, StringComparison.InvariantCultureIgnoreCase));

            return Optional<RunnerAction>.OfNullable(action);
        }

        private static bool IsRecordingSystemOk() =>
            !bool.Parse(CredentialsConfigFile.Get("tdl_require_rec", "true")) ||
            RecordingSystem.IsRunning();
    }
}
