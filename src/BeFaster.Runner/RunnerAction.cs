using TDL.Client.Actions;

namespace BeFaster.Runner
{
    public class RunnerAction
    {
        public string ShortName { get; }
        public string LongName { get; }
        public IClientAction ClientAction { get; }

        private RunnerAction(string shortName, string longName, IClientAction clientAction)
        {
            ShortName = shortName;
            LongName = longName;
            ClientAction = clientAction;
        }

        public static readonly RunnerAction GetNewRoundDescription = new RunnerAction("new", "getNewRoundDescription", ClientActions.Stop);
        public static readonly RunnerAction TestConnectivity = new RunnerAction("test", "testConnectivity", ClientActions.Stop);
        public static readonly RunnerAction DeployToProduction = new RunnerAction("deploy", "deployToProduction", ClientActions.Publish);

        public static readonly RunnerAction[] AllActions =
        {
            GetNewRoundDescription,
            TestConnectivity,
            DeployToProduction
        };
    }
}
