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

        //~~~~~~~~ The entry point ~~~~~~~~~

        public void Start(string[] args)
        {
            if (!RecordingSystem.IsRecordingSystemOk())
            {
                Console.WriteLine("Please run `record_screen_and_upload` before continuing.");
                return;
            }

            WindowsConsoleSupport.EnableColours();

            Console.WriteLine("Connecting to " + hostname);

            if (UseExperimentalFeature())
            {
                ExecuteServerActionFromUserInput(args);
            }
            else
            {
                ExecuteRunnerActionFromArgs(args);
            }

            bool holdAfterFinish = bool.Parse(CredentialsConfigFile.Get("tdl_hold_after_finish", "true"));
            if (holdAfterFinish)
            {
                Console.Write("\nPress any key to exit... ");
                Console.ReadKey();
            }
        }

        private static bool UseExperimentalFeature()
        {
            return bool.Parse(CredentialsConfigFile.Get("tdl_enable_experimental", "false"));
        }


        //~~~~~~~~ Runner Actions ~~~~~~~~~

        private void ExecuteRunnerActionFromArgs(string[] args)
        {
            var runnerAction = ExtractActionFrom(args).OrElse(defaultRunnerAction);
            ExecuteRunnerAction(runnerAction);
        }

        private static Optional<RunnerAction> ExtractActionFrom(IEnumerable<string> args)
        {
            var actionName = args.FirstOrDefault() ?? string.Empty;
            var action = RunnerAction.AllActions.FirstOrDefault(a =>
                a.LongName.Equals(actionName, StringComparison.InvariantCultureIgnoreCase));

            return Optional<RunnerAction>.OfNullable(action);
        }

        private void ExecuteRunnerAction(RunnerAction runnerAction)
        {
            Console.WriteLine("Chosen action is: " + runnerAction.LongName);

            var client = TdlClient.Build()
                .SetHostname(hostname)
                .SetUniqueId(username)
                .SetAuditStream(new ConsoleAuditStream())
                .Create();

            var processingRules = new ProcessingRules();

            processingRules
                .On("display_description")
                .Call(p => RoundManagement.SaveDescription(p[0], p[1]))
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
        }

        //~~~~~~~~ Server Actions ~~~~~~~~~

        private void ExecuteServerActionFromUserInput(string[] args)
        {
            try
            {
                var journeyId = CredentialsConfigFile.Get("tdl_journey_id");
                var useColours = bool.Parse(CredentialsConfigFile.Get("tdl_use_coloured_output", "true"));
                var challengeServerClient = new ChallengeServerClient(hostname, journeyId, useColours);

                var journeyProgress = challengeServerClient.GetJourneyProgress();
                Console.WriteLine(journeyProgress);
                
                var availableActions = challengeServerClient.GetAvailableActions();
                Console.WriteLine(availableActions);
                
                if (availableActions.Contains("No actions available.")) {
                    return;
                }
                
                var userInput = GetUserInput(args);
                if (userInput == null)
                {
                    Console.Error.WriteLine("No input stream detected. Please run this Solution on External Console.");
                    return;
                }
                
                //Obs: Deploy seems to be the only "special" action, everything else is driven by the server
                if (userInput.Equals("deploy")) {
                    // DEBT - the RecordingSystem.notifyEvent happens in executeRunnerAction, but once we migrate form the legacy system, we should move it outside for clarity
                    var runnerAction = RunnerAction.DeployToProduction;
                    ExecuteRunnerAction(runnerAction);
                }
        
                var actionFeedback = challengeServerClient.SendAction(userInput);
                Console.WriteLine(actionFeedback);

                var responseString = challengeServerClient.GetRoundDescription();
                RoundManagement.SaveDescription(
                    responseString,
                    lastFetchedRound => RecordingSystem.NotifyEvent(lastFetchedRound, RunnerAction.GetNewRoundDescription.ShortName)
                );
                
            } catch (ServerErrorException e) {
                Console.Error.WriteLine("Server experienced an error. Try again. " + e);
            } catch (OtherCommunicationException e) {
                Console.Error.WriteLine("Client threw an unexpected error. " + e);
            } catch (ClientErrorException e) {
                Console.Error.WriteLine("The client sent something the server didn't expect.");
                Console.WriteLine(e.Message);
            }
        }

        private string GetUserInput(string[] args)
        {
            return args.Length > 0 ? args[0] : ReadInputFromConsole();
        }

        private string ReadInputFromConsole()
        {
            return Console.ReadLine();
        }
    }
}